namespace Application.Features.Common.Notifications.Dtos;

public class NotificationListDto
{
    public List<NotificationDto> Notifications { get; set; } = new();
    public int TotalCount { get; set; }
    public int UnreadCount { get; set; }
    public int PageNumber { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}