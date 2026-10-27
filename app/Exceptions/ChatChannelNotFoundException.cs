namespace app.Exceptions;

public class ChatChannelNotFoundException(string? message = null): ArgumentException(message);