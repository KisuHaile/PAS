using MediatR;

namespace Application.Features.Requisition.ServiceRequests.Commands;

public class UpdateServiceRequestCommandHandler : IRequestHandler<UpdateServiceRequestCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public UpdateServiceRequestCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result> Handle(UpdateServiceRequestCommand request, CancellationToken cancellationToken)
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

        // Only allow updates if still pending
        if (serviceRequest.Status != "Pending")
        {
            return Result.Failure("Cannot update request that is not in Pending status.");
        }

        // Check if user is the requester
        if (serviceRequest.RequesterId != _currentUser.UserGuid)
        {
            return Result.Failure("You can only update your own requests.");
        }

        // Create a copy for event
        var oldRequest = new Domain.Requisition.ServiceRequest(
            serviceRequest.SRNumber,
            serviceRequest.RequesterId);

        // Update remarks
        if (!string.IsNullOrWhiteSpace(request.Remarks))
        {
            typeof(Domain.Requisition.ServiceRequest).GetProperty("Remarks")
                ?.SetValue(serviceRequest, request.Remarks);
        }

        // Remove existing details
        _context.SR_Details.RemoveRange(serviceRequest.Details);

        // Add new details
        foreach (var item in request.Items)
        {
            var itemMaster = await _context.ItemMasters
                .FirstOrDefaultAsync(i => i.Id == item.ItemId && !i.IsDeleted, cancellationToken);

            if (itemMaster == null)
            {
                return Result.Failure($"Item with ID {item.ItemId} not found.");
            }

            var detail = new Domain.Requisition.SR_Detail(
                serviceRequest.Id,
                item.ItemId,
                item.RequestedQty);

            if (item.PreferredShelfId.HasValue)
            {
                var shelf = await _context.ShelfLocations
                    .FirstOrDefaultAsync(s => s.Id == item.PreferredShelfId && !s.IsDeleted, cancellationToken);

                if (shelf == null)
                {
                    return Result.Failure($"Shelf with ID {item.PreferredShelfId} not found.");
                }

                typeof(Domain.Requisition.SR_Detail).GetProperty(nameof(Domain.Requisition.SR_Detail.ShelfId))
                    ?.SetValue(detail, item.PreferredShelfId);
            }

            if (!string.IsNullOrWhiteSpace(item.Notes))
            {
                typeof(Domain.Requisition.SR_Detail).GetProperty("Notes")
                    ?.SetValue(detail, item.Notes);
            }

            _context.SR_Details.Add(detail);
        }

        serviceRequest.MarkUpdated();

        // Create audit trail
        var auditTrail = new AuditTrail(
            _currentUser.UserGuid.Value,
            "UPDATE",
            nameof(Domain.Requisition.ServiceRequest),
            serviceRequest.Id);
        _context.AuditTrails.Add(auditTrail);

        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new EntityUpdatedEvent<Domain.Requisition.ServiceRequest>(serviceRequest, oldRequest, _currentUser.UserGuid), cancellationToken);

        return Result.Success();
    }
}