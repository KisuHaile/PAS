using Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PAS.API.Models.Responses;

namespace PAS.API.Filters;

public class ApiExceptionFilter : IExceptionFilter
{
    private readonly ILogger<ApiExceptionFilter> _logger;

    public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "API exception occurred");

        var response = new ErrorResponse
        {
            StatusCode = StatusCodes.Status500InternalServerError,
            Message = "An error occurred while processing your request."
        };

        switch (context.Exception)
        {
            case ValidationException validationException:
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = "Validation failed";
                response.Errors = validationException.Errors.SelectMany(x => x.Value).ToArray();
                break;

            case NotFoundException:
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = context.Exception.Message;
                break;

            case ForbiddenAccessException:
                response.StatusCode = StatusCodes.Status403Forbidden;
                response.Message = "Access denied";
                break;

            case BusinessRuleException:
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Message = context.Exception.Message;
                break;
        }

        context.Result = new ObjectResult(response)
        {
            StatusCode = response.StatusCode
        };
    }
}