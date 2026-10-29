namespace app.Exceptions;

public sealed class UserNotAuthorizedException(string? message = null): ArgumentException(message);