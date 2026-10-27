namespace app.Exceptions;

public sealed class UserDoesNotExistException(string? message = null): ArgumentException(message);