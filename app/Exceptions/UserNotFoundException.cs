namespace app.Exceptions;

public sealed class UserNotFoundException(string? message = null): ArgumentException(message);