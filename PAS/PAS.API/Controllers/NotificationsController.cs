using Application.Features.Common.Notifications.Commands.CreateNotification;
using Application.Features.Common.Notifications.Commands.MarkAllAsRead;
using Application.Features.Common.Notifications.Commands.MarkAsRead;
using Application.Features.Common.Notifications.Dtos;
using Application.Features.Common.Notifications.Queries.GetNotifications;
using Application.Features.Common.Notifications.Queries.GetUnreadCount;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAS.API.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace PAS.API.Controllers;

[Authorize]
public class NotificationsController : BaseApiController
{
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(ILogger<NotificationsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get user notifications with pagination
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Get user notifications")]
    [ProducesResponseType(typeof(ApiResponse<NotificationListDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<NotificationListDto>>> GetNotifications(
        [FromQuery] bool? showOnlyUnread = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetNotificationsQuery
        {
            ShowOnlyUnread = showOnlyUnread,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get unread notifications count
    /// </summary>
    [HttpGet("unread-count")]
    [SwaggerOperation(Summary = "Get unread notifications count")]
    [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<int>>> GetUnreadCount()
    {
        var query = new GetUnreadCountQuery();
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Create a notification for a user (used by admins to send messages
    /// such as password-reset instructions to a specific user).
    /// </summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create a notification for a user")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateNotification(CreateNotificationCommand command)
    {
        _logger.LogInformation("Creating notification for user: {UserId}", command.UserId);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Mark notification as read
    /// </summary>
    [HttpPost("{id}/read")]
    [SwaggerOperation(Summary = "Mark notification as read")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        _logger.LogInformation("Marking notification as read: {Id}", id);
        var command = new MarkAsReadCommand(id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Mark all notifications as read
    /// </summary>
    [HttpPost("read-all")]
    [SwaggerOperation(Summary = "Mark all notifications as read")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> MarkAllAsRead()
    {
        _logger.LogInformation("Marking all notifications as read");
        var command = new MarkAllAsReadCommand();
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}