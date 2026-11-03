using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace PAS.API.Hubs;

[Authorize]
public class NotificationHub : Hub
{
    private readonly ILogger<NotificationHub> _logger;

    public NotificationHub(ILogger<NotificationHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        if (userId != null)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
            _logger.LogInformation("User {UserId} connected to notification hub", userId);
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;
        if (userId != null)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user-{userId}");
            _logger.LogInformation("User {UserId} disconnected from notification hub", userId);
        }
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendNotification(Guid userId, string message, string type = "info")
    {
        await Clients.Group($"user-{userId}").SendAsync("ReceiveNotification", new
        {
            Id = Guid.NewGuid(),
            Message = message,
            Type = type,
            SentDate = DateTime.UtcNow
        });
    }

    public async Task SendNotificationToAll(string message, string type = "info")
    {
        await Clients.All.SendAsync("ReceiveNotification", new
        {
            Id = Guid.NewGuid(),
            Message = message,
            Type = type,
            SentDate = DateTime.UtcNow
        });
    }
}