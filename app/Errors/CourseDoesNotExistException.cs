namespace app.Errors;

public sealed class CourseDoesNotExistException(string message): ArgumentException(message);