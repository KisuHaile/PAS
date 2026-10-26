using Application.Common.Exceptions;
using Newtonsoft.Json;
using PAS.API.Models.Responses;

namespace PAS.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "Unhandled exception occurred: {Message}", exception.Message);

        context.Response.ContentType = "application/json";

        var response = new ErrorResponse
        {
            Message = exception.Message,
            Errors = new[] { exception.ToString() },
            StatusCode = exception switch
            {
                NotFoundException => StatusCodes.Status404NotFound,
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                ValidationException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            }
        };

        if (exception is ValidationException validationEx)
        {
            response.Errors = validationEx.Errors.SelectMany(kvp => kvp.Value).ToArray();
        }

        // Include inner exception for debugging
        if (exception.InnerException != null)
        {
            response.Message += $" | Inner: {exception.InnerException.Message}";
        }

        context.Response.StatusCode = response.StatusCode;

        return context.Response.WriteAsync(JsonConvert.SerializeObject(response));
    }
}