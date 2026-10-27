namespace app.Exceptions;

public class IllegalArgumentException(string? message = null): ArgumentException(message);