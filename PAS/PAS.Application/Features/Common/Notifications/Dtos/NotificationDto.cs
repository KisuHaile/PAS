using Application.Common.Mappings;
using AutoMapper;

namespace Application.Features.Common.Notifications.Dtos;

public class NotificationDto : IMapFrom<Domain.Common.Notification>
{
    public Guid Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime SentDate { get; set; }
    public string TimeAgo { get; set; } = string.Empty;

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Domain.Common.Notification, NotificationDto>()
            .ForMember(d => d.TimeAgo, opt => opt.Ignore());
    }
}