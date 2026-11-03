using MediatR;

namespace Application.Features.Requisition.ServiceRequests.Commands;

public class ApproveServiceRequestCommandHandler : IRequestHandler<ApproveServiceRequestCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public ApproveServiceRequestCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result> Handle(ApproveServiceRequestCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.UserGuid.HasValue)
        {
            return Result.Failure("User not authenticated.");
        }

        var serviceRequest = await _context.ServiceRequests
            .Include(s => s.Details)
            .FirstOrDefaultAsync(s => s.Id == request.Id && !s.IsDeleted, cancellationToken);

        if (serviceRequest == null)
        {
            throw new NotFoundException(nameof(Domain.Requisition.ServiceRequest), request.Id);
        }

        if (serviceRequest.Status != "Pending")
        {
            return Result.Failure($"Request is already {serviceRequest.Status.ToLower()}.");
        }

        // Create a copy for event
        var oldRequest = new Domain.Requisition.ServiceRequest(
            serviceRequest.SRNumber,
            serviceRequest.RequesterId);

        // Update status
        var newStatus = request.IsApproved ? "Approved" : "Rejected";
        typeof(Domain.Requisition.ServiceRequest).GetProperty(nameof(Domain.Requisition.ServiceRequest.Status))
            ?.SetValue(serviceRequest, newStatus);

        if (request.IsApproved)
        {
            serviceRequest.Approve();
        }

        serviceRequest.MarkUpdated();

        // Create audit trail
        var auditTrail = new AuditTrail(
            _currentUser.UserGuid.Value,
            request.IsApproved ? "APPROVE" : "REJECT",
            nameof(Domain.Requisition.ServiceRequest),
            serviceRequest.Id);
        _context.AuditTrails.Add(auditTrail);

        await _context.SaveChangesAsync(cancellationToken);

        // Notify requester
        await NotifyRequesterAsync(serviceRequest, request, cancellationToken);

        await _mediator.Publish(new EntityUpdatedEvent<Domain.Requisition.ServiceRequest>(serviceRequest, oldRequest, _currentUser.UserGuid), cancellationToken);

        return Result.Success();
    }

    private async Task NotifyRequesterAsync(
        Domain.Requisition.ServiceRequest serviceRequest,
        ApproveServiceRequestCommand request,
        CancellationToken cancellationToken)
    {
        var status = request.IsApproved ? "approved" : "rejected";
        var message = $"Your service request {serviceRequest.SRNumber} has been {status}.";

        if (!string.IsNullOrWhiteSpace(request.Remarks))
        {
            message += $" Remarks: {request.Remarks}";
        }

        var notification = new Notification(serviceRequest.RequesterId, message);
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync(cancellationToken);
    }
}