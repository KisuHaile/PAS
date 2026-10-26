using MediatR;

namespace Application.Features.Requisition.StoreIssueVouchers.Commands;

public class CreateStoreIssueVoucherCommandHandler : IRequestHandler<CreateStoreIssueVoucherCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public CreateStoreIssueVoucherCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result<Guid>> Handle(CreateStoreIssueVoucherCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.UserGuid.HasValue)
        {
            return Result<Guid>.Failure("User not authenticated.");
        }

        // Get service request with details
        var serviceRequest = await _context.ServiceRequests
            .Include(s => s.Details)
            .FirstOrDefaultAsync(s => s.Id == request.SRId && !s.IsDeleted, cancellationToken);

        if (serviceRequest == null)
        {
            return Result<Guid>.Failure($"Service request with ID {request.SRId} not found.");
        }

        if (serviceRequest.Status != "Approved")
        {
            return Result<Guid>.Failure("Service request must be approved before issuing items.");
        }

        // Generate SIV number
        var sivCount = await _context.StoreIssueVouchers.CountAsync(cancellationToken) + 1;
        var sivNumber = $"SIV-{DateTime.Now:yyyyMMdd}-{sivCount:D4}";

        // Create SIV
        var siv = new Domain.Requisition.StoreIssueVoucher(
            request.SRId,
            _currentUser.UserGuid.Value,
            request.RecipientSignature);

        typeof(Domain.Requisition.StoreIssueVoucher).GetProperty(nameof(Domain.Requisition.StoreIssueVoucher.SIVNumber))
            ?.SetValue(siv, sivNumber);

        if (!string.IsNullOrWhiteSpace(request.RecipientName))
        {
            typeof(Domain.Requisition.StoreIssueVoucher).GetProperty("RecipientName")
                ?.SetValue(siv, request.RecipientName);
        }

        if (!string.IsNullOrWhiteSpace(request.Remarks))
        {
            typeof(Domain.Requisition.StoreIssueVoucher).GetProperty("Remarks")
                ?.SetValue(siv, request.Remarks);
        }

        _context.StoreIssueVouchers.Add(siv);

        // Process each issued item
        var allIssued = true;
        foreach (var item in request.Items)
        {
            var srDetail = serviceRequest.Details?.FirstOrDefault(d => d.Id == item.SRDetailId);
            if (srDetail == null)
            {
                return Result<Guid>.Failure($"Service request detail {item.SRDetailId} not found.");
            }

            if (item.IssuedQty > srDetail.RequestedQty - srDetail.IssuedQty)
            {
                return Result<Guid>.Failure(
                    $"Issued quantity {item.IssuedQty} exceeds remaining requested quantity {srDetail.RequestedQty - srDetail.IssuedQty} for item.");
            }

            // Update SR detail
            srDetail.Issue(srDetail.IssuedQty + item.IssuedQty);

            // Update inventory stock
            if (item.ShelfId.HasValue)
            {
                var inventoryStock = await _context.InventoryStocks
                    .FirstOrDefaultAsync(i => i.ItemId == srDetail.ItemId &&
                                             i.ShelfId == item.ShelfId &&
                                             !i.IsDeleted, cancellationToken);

                if (inventoryStock == null)
                {
                    return Result<Guid>.Failure($"No stock found for item at the selected shelf.");
                }

                if (inventoryStock.CurrentQuantity - inventoryStock.ReservedQuantity < item.IssuedQty)
                {
                    return Result<Guid>.Failure($"Insufficient stock. Available: {inventoryStock.CurrentQuantity - inventoryStock.ReservedQuantity}");
                }

                // Reduce current quantity
                typeof(InventoryStock).GetProperty(nameof(InventoryStock.CurrentQuantity))
                    ?.SetValue(inventoryStock, inventoryStock.CurrentQuantity - item.IssuedQty);

                // Create stock ledger entry
                var ledger = new StockLedger(
                    srDetail.ItemId,
                    item.ShelfId.Value,
                    -item.IssuedQty,
                    "ISSUED",
                    siv.Id);

                if (!string.IsNullOrWhiteSpace(item.BatchNumber))
                {
                    typeof(StockLedger).GetProperty("BatchNumber")?.SetValue(ledger, item.BatchNumber);
                }

                if (item.ExpiryDate.HasValue)
                {
                    typeof(StockLedger).GetProperty("ExpiryDate")?.SetValue(ledger, item.ExpiryDate);
                }

                _context.StockLedgers.Add(ledger);
            }

            if (srDetail.IssuedQty < srDetail.RequestedQty)
            {
                allIssued = false;
            }
        }

        // Update service request status
        var newStatus = allIssued ? "Completed" : "Issued";
        typeof(Domain.Requisition.ServiceRequest).GetProperty(nameof(Domain.Requisition.ServiceRequest.Status))
            ?.SetValue(serviceRequest, newStatus);
        serviceRequest.MarkUpdated();

        // Create audit trail
        var auditTrail = new AuditTrail(
            _currentUser.UserGuid.Value,
            "CREATE",
            nameof(Domain.Requisition.StoreIssueVoucher),
            siv.Id);
        _context.AuditTrails.Add(auditTrail);

        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new EntityCreatedEvent<Domain.Requisition.StoreIssueVoucher>(siv, _currentUser.UserGuid), cancellationToken);

        return Result<Guid>.Success(siv.Id);
    }
}