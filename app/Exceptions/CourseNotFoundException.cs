namespace app.Exceptions;

public sealed class CourseNotFoundException(string? message = null): ArgumentException(message);