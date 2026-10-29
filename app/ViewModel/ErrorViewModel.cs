namespace app.Models;

public class ErrorViewModel()
{
    public string ErrorDescription { get; init; } = string.Empty;
    public int ErrorCode { get; init; }
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}