namespace app.Errors;

public sealed class CourseDoesNotExistException(string? message = null): ArgumentException(message);