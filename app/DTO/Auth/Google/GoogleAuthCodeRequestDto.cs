namespace app.DTO.Auth.Google;

public class GoogleAuthCodeRequestDto(string code)
{
    private string client_id { get; } = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID") ?? string.Empty;
    private string client_secret { get; } = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET") ?? string.Empty;
    private string code { get; init; } = code;
    private const string grant_type = "authorization_code";
    private string redirect_uri { get; set; } =  Environment.GetEnvironmentVariable("GOOGLE_REDIRECT_URL") ?? string.Empty;
}