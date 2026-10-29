namespace app.Exceptions;

public sealed class UserAlreadyExistsException(string? message = null): ArgumentException(message);