namespace app.Errors;

public class UserAlreadyExistsException(string? message = null): ArgumentException(message);