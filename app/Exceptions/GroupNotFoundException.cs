namespace app.Exceptions;

public class GroupNotFoundException(string? message = null): ArgumentException(message);