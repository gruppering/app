namespace app.Errors;

public class UserDoesNotExistException(string? message = null): ArgumentException(message);