namespace app.Exceptions;

public class PasswordNotMeetingCriteriaException(string? message = null): ArgumentException(message);