using MediatR;

namespace Application.Features.Common.Notifications.Commands.MarkAsRead;

public class MarkAsReadCommandHandler : IRequestHandler<MarkAsReadCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public MarkAsReadCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(MarkAsReadCommand request, CancellationToken cancellationToken)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == request.Id && !n.IsDeleted, cancellationToken);

        if (notification == null)
        {
            throw new NotFoundException(nameof(Domain.Common.Notification), request.Id);
        }

        if (notification.UserId != _currentUser.UserGuid)
        {
            return Result.Failure("You can only mark your own notifications as read.");
        }

        notification.MarkAsRead();

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}