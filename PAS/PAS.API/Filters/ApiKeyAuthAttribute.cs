using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PAS.API.Models.Responses;

namespace PAS.API.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiKeyAuthAttribute : Attribute, IAsyncAuthorizationFilter
{
    private const string ApiKeyHeaderName = "X-API-Key";
    private const string ApiKeyValue = "your-api-key-here"; // In production, store in configuration

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
        {
            context.Result = new UnauthorizedObjectResult(new ErrorResponse
            {
                StatusCode = StatusCodes.Status401Unauthorized,
                Message = "API Key is missing",
                Timestamp = DateTime.UtcNow
            });
            return;
        }

        if (!ApiKeyValue.Equals(extractedApiKey))
        {
            context.Result = new UnauthorizedObjectResult(new ErrorResponse
            {
                StatusCode = StatusCodes.Status401Unauthorized,
                Message = "Invalid API Key",
                Timestamp = DateTime.UtcNow
            });
            return;
        }

        await Task.CompletedTask;
    }
}