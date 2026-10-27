namespace app.Exceptions;

public class UserDoesNotExistException(string? message = null): ArgumentException(message);