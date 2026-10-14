namespace app.DTO.Auth.GitHub;

public class GitHubAuthCodeRequestDto(string code,string redirectUri)
{
    public string client_id { get; set; } = Environment.GetEnvironmentVariable("GITHUB_CLIENT_ID") ?? string.Empty;
    public string client_secret { get; set; } = Environment.GetEnvironmentVariable("GITHUB_CLIENT_SECRET") ?? string.Empty;
    public string code { get; init; } = code;
    public string redirect_uri { get; set; } =  redirectUri;
}