namespace app.Exceptions;

public sealed class PasswordNotMeetingCriteriaException(string? message = null): ArgumentException(message);