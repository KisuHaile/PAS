using MediatR;

namespace Application.Features.Requisition.ServiceRequests.Commands;

public class CreateServiceRequestCommandHandler : IRequestHandler<CreateServiceRequestCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public CreateServiceRequestCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result<Guid>> Handle(CreateServiceRequestCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.UserGuid.HasValue)
        {
            return Result<Guid>.Failure("User not authenticated.");
        }

        // Generate SR number
        var srCount = await _context.ServiceRequests.CountAsync(cancellationToken) + 1;
        var srNumber = $"SR-{DateTime.Now:yyyyMMdd}-{srCount:D4}";

        // Create service request
        var serviceRequest = new Domain.Requisition.ServiceRequest(
            srNumber,
            _currentUser.UserGuid.Value);

        if (!string.IsNullOrWhiteSpace(request.Remarks))
        {
            typeof(Domain.Requisition.ServiceRequest).GetProperty("Remarks")
                ?.SetValue(serviceRequest, request.Remarks);
        }

        _context.ServiceRequests.Add(serviceRequest);

        // Add details
        foreach (var item in request.Items)
        {
            var itemMaster = await _context.ItemMasters
                .FirstOrDefaultAsync(i => i.Id == item.ItemId && !i.IsDeleted, cancellationToken);

            if (itemMaster == null)
            {
                return Result<Guid>.Failure($"Item with ID {item.ItemId} not found.");
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
                    return Result<Guid>.Failure($"Shelf with ID {item.PreferredShelfId} not found.");
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

        // Create audit trail
        var auditTrail = new AuditTrail(
            _currentUser.UserGuid.Value,
            "CREATE",
            nameof(Domain.Requisition.ServiceRequest),
            serviceRequest.Id);
        _context.AuditTrails.Add(auditTrail);

        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new EntityCreatedEvent<Domain.Requisition.ServiceRequest>(serviceRequest, _currentUser.UserGuid), cancellationToken);

        return Result<Guid>.Success(serviceRequest.Id);
    }
}
