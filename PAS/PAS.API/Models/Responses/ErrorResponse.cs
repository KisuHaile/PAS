namespace PAS.API.Models.Responses;

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string[]? Errors { get; set; }
    public string? StackTrace { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}