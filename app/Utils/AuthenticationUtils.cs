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

    public static async Task<JwtSecurityToken> GetGoogleToken(string code, string redirectUri)
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