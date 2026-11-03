using Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace PAS.API.BackgroundServices;

public class NotificationCleanupService : BackgroundService
{
    private readonly ILogger<NotificationCleanupService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public NotificationCleanupService(ILogger<NotificationCleanupService> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Notification cleanup service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            await CleanupOldNotifications();
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }

    public async Task CleanupOldNotifications()
    {
        _logger.LogInformation("Cleaning up old notifications");

        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

        var cutoffDate = DateTime.UtcNow.AddMonths(-3);
        var oldNotifications = await context.Notifications
            .Where(n => n.SentDate < cutoffDate && n.IsRead)
            .ToListAsync();

        foreach (var notification in oldNotifications)
        {
            notification.SoftDelete();
        }

        await context.SaveChangesAsync();
        _logger.LogInformation("Cleaned up {Count} old notifications", oldNotifications.Count);
    }
}