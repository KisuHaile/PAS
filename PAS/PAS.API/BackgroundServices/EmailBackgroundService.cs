using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PAS.API.BackgroundServices
{
    public class EmailBackgroundService : BackgroundService
    {
        private readonly ILogger<EmailBackgroundService> _logger;

        public EmailBackgroundService(ILogger<EmailBackgroundService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Email background service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                await SendDailyDigest();
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }

        public Task SendDailyDigest()
        {
            _logger.LogInformation("Sending daily digest emails");
            return Task.CompletedTask;
        }

        public Task SendWeeklyReport()
        {
            _logger.LogInformation("Sending weekly report emails");
            return Task.CompletedTask;
        }
    }
}