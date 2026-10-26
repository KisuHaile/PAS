using Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Application.Common.Behaviours
{
    public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly Stopwatch _timer;
        private readonly ILogger<TRequest> _logger;
        private readonly ICurrentUserService _currentUserService;

        public PerformanceBehaviour(ILogger<TRequest> logger, ICurrentUserService currentUserService)
        {
            _timer = new Stopwatch();
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            _timer.Start();

            var response = await next();

            _timer.Stop();

            var elapsedMilliseconds = _timer.ElapsedMilliseconds;

            if (elapsedMilliseconds > 500) // Warning if > 500ms
            {
                var requestName = typeof(TRequest).Name;
                var userId = _currentUserService.UserId?.ToString() ?? "Anonymous";
                var userName = _currentUserService.Username ?? "Anonymous";

                _logger.LogWarning("Property Automation Long Running Request: {Name} ({ElapsedMilliseconds} ms) {@UserId} {@UserName} {@Request}",
                    requestName, elapsedMilliseconds, userId, userName, request);
            }

            return response;
        }
    }
}