namespace PAS.API.Models.Responses;

public class ValidationErrorResponse : ErrorResponse
{
    public Dictionary<string, string[]>? ValidationErrors { get; set; }
}