namespace app.DTO.Auth.Google;

public class GoogleAuthCodeRequestDto(string code)
{
    public string client_id { get; set; } = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID") ?? string.Empty;
    public string client_secret { get; set; } = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET") ?? string.Empty;
    public string code { get; init; } = code;
    public string grant_type => "authorization_code";
    public string redirect_uri { get; set; } =  Environment.GetEnvironmentVariable("GOOGLE_REDIRECT_URL") ?? string.Empty;
}