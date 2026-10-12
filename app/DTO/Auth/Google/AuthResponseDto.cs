namespace app.DTO.Auth.Google;

public record AuthResponseDto(string access_token,int expires_in,string scope,string token_type,string id_token);