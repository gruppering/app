namespace app.Exceptions;

public class MessageNotFoundException(string? message = null): ArgumentException(message);