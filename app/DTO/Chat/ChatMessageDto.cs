namespace app.DTO.Chat;

public record ChatMessageDto(Guid Id,Guid UserId,string Username,string Message);