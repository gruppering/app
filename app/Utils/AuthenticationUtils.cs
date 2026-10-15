using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using app.DTO.Auth.GitHub;
using app.DTO.Auth.Google;

namespace app.Utils;

public static class AuthenticationUtils
{

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
        HttpClient httpClient = new();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        // Generates a new request body.
        var body = JsonSerializer.Serialize(
            new GoogleAuthCodeRequestDto(code,redirectUri));
        var content = new StringContent(body,Encoding.UTF8,"application/json");
        
        // Sends the post request to google's servers, and gets the token in return.
        var response = await httpClient.PostAsync(" https://oauth2.googleapis.com/token", content);

        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        
        return new JwtSecurityTokenHandler().ReadJwtToken(authResponse!.id_token);
    }
    
    /// <summary>
    /// Gets a GitHub JWT with the given access code and redirect uri.
    /// </summary>
    /// <param name="code">The access code acquired when user logs in via GitHub</param>
    /// <param name="redirectUri">The redirect uri to use, dynamically generated.</param>
    /// <returns>A JwtSecurityToken, containing all the important claims necessary to either log in or sign up the user.</returns>
    public static async Task<GithubUserInfoDto> GetGitHubUserInformation(string code, string redirectUri)
    {
        HttpClient httpClient = new();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd("Gruppering Web Server/Kestrel");

        // Generates a new request body.
        var body = JsonSerializer.Serialize(
            new GitHubAuthCodeRequestDto(code,redirectUri));
        var content = new StringContent(body,Encoding.UTF8,"application/json");
        
        // Sends the post request to google's servers, and gets the token in return.
        var response = await httpClient.PostAsync(" https://github.com/login/oauth/access_token", content);
        
        var authResponse = await response.Content.ReadFromJsonAsync<GitHubAuthCodeResponseDto>();
        
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponse!.access_token);

        // In contrast to the Google implementation, we actually have to get the user info from the api, as no jwt is returned above.
        var userInfoResponse = await httpClient.GetAsync("https://api.github.com/user");
        var emailInfoResponse = await httpClient.GetAsync("https://api.github.com/user/emails");
        
        var userInfo = await userInfoResponse.Content.ReadFromJsonAsync<GithubUserInfoDto>();
        var userEmails = await emailInfoResponse.Content.ReadFromJsonAsync<List<GithubUserEmailDto>>();

        // As for some reason, the email field in the user endpoint is always null, so we have to fetch and set the email separately.
        userInfo!.email = userEmails!.First().email; 

        return userInfo;
    }
}