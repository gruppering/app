namespace app.Exceptions;

public sealed class ChatChannelNotFoundException(string? message = null): ArgumentException(message);