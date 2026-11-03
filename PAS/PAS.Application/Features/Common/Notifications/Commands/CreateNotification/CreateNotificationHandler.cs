using Application.Events;
using Domain.Common;
using MediatR;

namespace Application.Features.Common.Notifications.Commands.CreateNotification;

public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public CreateNotificationCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result<Guid>> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId == Guid.Empty)
        {
            return Result<Guid>.Failure("UserId is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return Result<Guid>.Failure("Message is required.");
        }

        var notification = new Notification(request.UserId, request.Message);
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(
            new EntityCreatedEvent<Notification>(notification, _currentUser.UserGuid),
            cancellationToken);

        return Result<Guid>.Success(notification.Id);
    }
}
