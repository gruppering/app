namespace app.Exceptions;

public sealed class GroupNotFoundException(string? message = null): ArgumentException(message);