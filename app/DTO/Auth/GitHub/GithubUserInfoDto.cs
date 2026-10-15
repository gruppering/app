namespace app.DTO.Auth.GitHub;

public class GithubUserInfoDto
{
    public required string name { get; set; }
    public required string avatar_url { get; set; }
    public required string email { get; set; }
}