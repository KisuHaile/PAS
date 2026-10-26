using Application.Common.Interfaces;
using Application.Common.Security;
using Application.Common.Exceptions;
using MediatR;
using System.Reflection;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Behaviours
{
    public class AuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ICurrentUserService _currentUserService;

        public AuthorizationBehaviour(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var authorizeAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>();

            if (authorizeAttributes.Any())
            {
                // Must be authenticated
                if (!_currentUserService.IsAuthenticated)
                {
                    throw new UnauthorizedAccessException("User is not authenticated");
                }

                // Role-based authorization
                var authorizeAttributesWithRoles = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Roles));

                if (authorizeAttributesWithRoles.Any())
                {
                    var authorized = false;

                    foreach (var roles in authorizeAttributesWithRoles.Select(a => a.Roles.Split(',')))
                    {
                        foreach (var role in roles)
                        {
                            if (_currentUserService.IsInRole(role.Trim()))
                            {
                                authorized = true;
                                break;
                            }
                        }
                    }

                    // Must be in at least one of the listed roles
                    if (!authorized)
                    {
                        throw new ForbiddenAccessException("User does not have the required role");
                    }
                }

                // Policy-based authorization
                var authorizeAttributesWithPolicies = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Policy));
                if (authorizeAttributesWithPolicies.Any())
                {
                    foreach (var policy in authorizeAttributesWithPolicies.Select(a => a.Policy))
                    {
                        // Implement policy check here
                        var authorized = await AuthorizeAsync(policy);
                        if (!authorized)
                        {
                            throw new ForbiddenAccessException($"User does not have the required policy: {policy}");
                        }
                    }
                }
            }

            return await next();
        }

        private Task<bool> AuthorizeAsync(string policy)
        {
            // Implement policy authorization logic
            return Task.FromResult(true);
        }
    }
}