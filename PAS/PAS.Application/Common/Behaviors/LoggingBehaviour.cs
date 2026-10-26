using Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviours
{
    public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ILogger _logger;
        private readonly ICurrentUserService _currentUserService;

        public LoggingBehaviour(ILogger<TRequest> logger, ICurrentUserService currentUserService)
        {
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var userId = _currentUserService.UserId?.ToString() ?? "Anonymous";
            var userName = _currentUserService.Username ?? "Anonymous";

            _logger.LogInformation("Property Automation Request: {Name} {@UserId} {@UserName} {@Request}",
                requestName, userId, userName, request);

            var response = await next();

            _logger.LogInformation("Property Automation Response: {Name} {@Response}", requestName, response);

            return response;
        }
    }
}