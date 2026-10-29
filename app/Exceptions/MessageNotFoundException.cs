namespace app.Exceptions;

public sealed class MessageNotFoundException(string? message = null): ArgumentException(message);