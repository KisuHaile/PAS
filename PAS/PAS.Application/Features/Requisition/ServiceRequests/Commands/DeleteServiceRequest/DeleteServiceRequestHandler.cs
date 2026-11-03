using MediatR;
using System.Data;

namespace Application.Features.Requisition.ServiceRequests.Commands;

public class DeleteServiceRequestCommandHandler : IRequestHandler<DeleteServiceRequestCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public DeleteServiceRequestCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result> Handle(DeleteServiceRequestCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.UserGuid.HasValue)
        {
            return Result.Failure("User not authenticated.");
        }

        var serviceRequest = await _context.ServiceRequests
            .Include(s => s.Details)
            .Include(s => s.StoreIssueVoucher)
            .FirstOrDefaultAsync(s => s.Id == request.Id && !s.IsDeleted, cancellationToken);

        if (serviceRequest == null)
        {
            throw new NotFoundException(nameof(Domain.Requisition.ServiceRequest), request.Id);
        }

        // Only allow deletion if pending
        if (serviceRequest.Status != "Pending")
        {
            return Result.Failure("Cannot delete request that has been processed.");
        }

        // Check if user is the requester or has admin rights
        if (serviceRequest.RequesterId != _currentUser.UserGuid &&
            !_currentUser.IsInRole(Roles.Admin))
        {
            return Result.Failure("You don't have permission to delete this request.");
        }

        // Soft delete details
        if (serviceRequest.Details?.Any() == true)
        {
            foreach (var detail in serviceRequest.Details.Where(d => !d.IsDeleted))
            {
                detail.SoftDelete();
            }
        }

        // Soft delete the request
        serviceRequest.SoftDelete();

        // Create audit trail
        var auditTrail = new AuditTrail(
            _currentUser.UserGuid.Value,
            "DELETE",
            nameof(Domain.Requisition.ServiceRequest),
            serviceRequest.Id);
        _context.AuditTrails.Add(auditTrail);

        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new EntityDeletedEvent<Domain.Requisition.ServiceRequest>(serviceRequest, _currentUser.UserGuid), cancellationToken);

        return Result.Success();
    }
}