using Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PAS.API.Models.Responses;

namespace PAS.API.Filters;

public class AuthorizeFilter : IAsyncAuthorizationFilter
{
    private readonly ICurrentUserService _currentUserService;

    public AuthorizeFilter(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var allowAnonymous = context.ActionDescriptor.EndpointMetadata
            .Any(em => em is AllowAnonymousAttribute);

        if (allowAnonymous)
        {
            return;
        }

        if (!_currentUserService.IsAuthenticated)
        {
            context.Result = new UnauthorizedObjectResult(new ErrorResponse
            {
                StatusCode = StatusCodes.Status401Unauthorized,
                Message = "Unauthorized access",
                Timestamp = DateTime.UtcNow
            });
            return;
        }

        // Check for specific permissions if needed
        var authorizeAttributes = context.ActionDescriptor.EndpointMetadata
            .Where(em => em is AuthorizeAttribute)
            .Cast<AuthorizeAttribute>();

        foreach (var attribute in authorizeAttributes)
        {
            if (!string.IsNullOrEmpty(attribute.Roles))
            {
                var requiredRoles = attribute.Roles.Split(',');
                if (!requiredRoles.Any(r => _currentUserService.IsInRole(r.Trim())))
                {
                    context.Result = new ForbidResult();
                    return;
                }
            }
        }

        await Task.CompletedTask;
    }
}