using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using app.DTO.Auth.Google;

namespace app.Utils;

public static class AuthenticationUtils
{
    private static readonly HttpClient HttpClient = new ();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="claims"></param>
    /// <returns></returns>
    public static ClaimsPrincipal GenerateClaimsPrincipal(List<Claim> claims)
    {
        return new ClaimsPrincipal(new ClaimsIdentity(claims, "cookie"));
        
    }

    /// <summary>
    /// Gets a Google JWT with the given access code and redirect uri.
    /// </summary>
    /// <param name="code">The access code acquired when user logs in via Google</param>
    /// <param name="redirectUri">The redirect uri to use, dynamically generated.</param>
    /// <returns>A JwtSecurityToken, containing all the important claims necessary to either log in or sign up the user.</returns>
    public static async Task<JwtSecurityToken> GetGoogleJwt(string code, string redirectUri)
    {
        HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        // Generates a new request body.
        var body = JsonSerializer.Serialize(
            new GoogleAuthCodeRequestDto(code,redirectUri));
        var content = new StringContent(body,Encoding.UTF8,"application/json");
        
        // Sends the post request to google's servers, and gets the token in return.
        var response = await HttpClient.PostAsync(" https://oauth2.googleapis.com/token", content);

        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        
        return new JwtSecurityTokenHandler().ReadJwtToken(authResponse!.id_token);
    }
}