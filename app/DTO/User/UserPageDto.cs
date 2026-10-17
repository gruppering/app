namespace app.DTO.User;
/*
 * This DTO is still hypothetical and can of course change later depending on what info should be displayed on-
 * the users profile page 
 */
public record UserPageDto(string username, string email, string? displayName);