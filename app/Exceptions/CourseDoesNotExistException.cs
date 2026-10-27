namespace app.Exceptions;

public sealed class CourseDoesNotExistException(string? message = null): ArgumentException(message);