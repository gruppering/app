namespace app.Exceptions;

public class UserAlreadyExistsException(string? message = null): ArgumentException(message);