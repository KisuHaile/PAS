using Application.Common.Security;
using MediatR;

namespace Application.Features.Requisition.ServiceRequests.Commands.RejectServiceRequest;

[Authorize(Permissions = Permissions.Requisitions.Approve)]
public record RejectServiceRequestCommand : IRequest<Result>
{
    public Guid Id { get; init; }
    public string? Remarks { get; init; }
}

public class RejectServiceRequestCommandHandler : IRequestHandler<RejectServiceRequestCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public RejectServiceRequestCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(RejectServiceRequestCommand request, CancellationToken cancellationToken)
    {
        var sr = await _context.ServiceRequests.FirstOrDefaultAsync(s => s.Id == request.Id && !s.IsDeleted, cancellationToken);
        if (sr == null) return Result.Failure("Service request not found.");

        typeof(Domain.Requisition.ServiceRequest).GetProperty(nameof(Domain.Requisition.ServiceRequest.Status))?.SetValue(sr, "Rejected");
        sr.MarkUpdated();

        _context.AuditTrails.Add(new Domain.Common.AuditTrail(_currentUser.UserId ?? Guid.Empty, "REJECT", nameof(Domain.Requisition.ServiceRequest), sr.Id));
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
