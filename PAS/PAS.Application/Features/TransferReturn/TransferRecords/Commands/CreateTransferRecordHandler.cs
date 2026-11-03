using MediatR;

namespace Application.Features.TransferReturn.TransferRecords.Commands;

public class CreateTransferRecordCommandHandler : IRequestHandler<CreateTransferRecordCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public CreateTransferRecordCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result<Guid>> Handle(CreateTransferRecordCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.UserGuid.HasValue)
        {
            return Result<Guid>.Failure("User not authenticated.");
        }

        if (request.Quantity <= 0)
        {
            return Result<Guid>.Failure("Quantity must be greater than zero.");
        }

        // Validate item exists
        var item = await _context.ItemMasters
            .FirstOrDefaultAsync(i => i.Id == request.ItemId && !i.IsDeleted, cancellationToken);

        if (item == null)
        {
            return Result<Guid>.Failure($"Item with ID {request.ItemId} not found.");
        }

        // Validate destination location
        var toLocation = await _context.PropertyLocations
            .FirstOrDefaultAsync(l => l.Id == request.ToLocationId && !l.IsDeleted, cancellationToken);

        if (toLocation == null)
        {
            return Result<Guid>.Failure($"Destination location with ID {request.ToLocationId} not found.");
        }

        // Validate destination shelf if provided
        if (request.ToShelfId.HasValue)
        {
            var toShelf = await _context.ShelfLocations
                .Include(s => s.Warehouse)
                .FirstOrDefaultAsync(s => s.Id == request.ToShelfId && !s.IsDeleted, cancellationToken);

            if (toShelf == null)
            {
                return Result<Guid>.Failure($"Destination shelf with ID {request.ToShelfId} not found.");
            }

            // Check if shelf belongs to the destination location (if it's a warehouse location)
            if (toLocation.LocationType == "Warehouse" && toShelf.WarehouseId != toLocation.Id)
            {
                return Result<Guid>.Failure("Destination shelf does not belong to the destination location.");
            }
        }

        // Find source location and shelf from current inventory
        var inventoryStock = await _context.InventoryStocks
            .Include(i => i.ShelfLocation)
                .ThenInclude(s => s.Warehouse)
            .FirstOrDefaultAsync(i => i.ItemId == request.ItemId &&
                                     i.CurrentQuantity >= request.Quantity &&
                                     !i.IsDeleted, cancellationToken);

        if (inventoryStock == null)
        {
            return Result<Guid>.Failure("Insufficient stock available for transfer.");
        }

        if (inventoryStock.ShelfLocation?.Warehouse == null)
        {
            return Result<Guid>.Failure("Source inventory shelf/warehouse is invalid.");
        }

        // Resolve source PropertyLocation (TransferRecord FK expects PropertyLocations, not Warehouses)
        var sourceWarehouse = inventoryStock.ShelfLocation.Warehouse;
        var fromLocation = await _context.PropertyLocations
            .FirstOrDefaultAsync(l => !l.IsDeleted
                                   && l.LocationType == "Warehouse"
                                   && l.Name == sourceWarehouse.WarehouseName, cancellationToken);

        if (fromLocation == null)
        {
            fromLocation = new Domain.PropertyManagement.PropertyLocation(
                sourceWarehouse.WarehouseName,
                "Warehouse");
            _context.PropertyLocations.Add(fromLocation);
            await _context.SaveChangesAsync(cancellationToken);
        }

        // Generate transfer number
        var transferCount = await _context.TransferRecords.CountAsync(cancellationToken) + 1;
        var transferNumber = $"TRF-{DateTime.Now:yyyyMMdd}-{transferCount:D4}";

        // Create transfer record
        var transferRecord = new Domain.TransferReturn.TransferRecord(
            request.ItemId,
            fromLocation.Id,
            request.ToLocationId,
            request.Quantity);

        typeof(Domain.TransferReturn.TransferRecord).GetProperty("TransferNumber")?.SetValue(transferRecord, transferNumber);
        typeof(Domain.TransferReturn.TransferRecord).GetProperty("FromShelfId")?.SetValue(transferRecord, inventoryStock.ShelfId);
        typeof(Domain.TransferReturn.TransferRecord).GetProperty("ToShelfId")?.SetValue(transferRecord, request.ToShelfId);
        typeof(Domain.TransferReturn.TransferRecord).GetProperty("InitiatedById")?.SetValue(transferRecord, _currentUser.UserGuid);
        typeof(Domain.TransferReturn.TransferRecord).GetProperty("Status")?.SetValue(transferRecord, "Pending");
        typeof(Domain.TransferReturn.TransferRecord).GetProperty("Reason")?.SetValue(transferRecord, request.Reason);
        typeof(Domain.TransferReturn.TransferRecord).GetProperty("Remarks")?.SetValue(transferRecord, request.Remarks);
        typeof(Domain.TransferReturn.TransferRecord).GetProperty("Reference")?.SetValue(transferRecord, request.Reference);
        typeof(Domain.TransferReturn.TransferRecord).GetProperty("BatchNumber")?.SetValue(transferRecord, request.BatchNumber);
        typeof(Domain.TransferReturn.TransferRecord).GetProperty("ExpiryDate")?.SetValue(transferRecord, request.ExpiryDate);

        _context.TransferRecords.Add(transferRecord);

        // Reserve stock for transfer
        typeof(Domain.Storage.InventoryStock).GetProperty(nameof(Domain.Storage.InventoryStock.ReservedQuantity))
            ?.SetValue(inventoryStock, inventoryStock.ReservedQuantity + request.Quantity);

        // Create stock ledger entry for transfer reservation
        var ledger = new Domain.Storage.StockLedger(
            request.ItemId,
            inventoryStock.ShelfId,
            -request.Quantity,
            "TRANSFER_RESERVED",
            transferRecord.Id);

        typeof(Domain.Storage.StockLedger).GetProperty("BatchNumber")?.SetValue(ledger, request.BatchNumber);
        typeof(Domain.Storage.StockLedger).GetProperty("ExpiryDate")?.SetValue(ledger, request.ExpiryDate);
        _context.StockLedgers.Add(ledger);

        // Create audit trail
        var auditTrail = new AuditTrail(
            _currentUser.UserGuid.Value,
            "CREATE",
            nameof(Domain.TransferReturn.TransferRecord),
            transferRecord.Id);
        _context.AuditTrails.Add(auditTrail);

        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new EntityCreatedEvent<Domain.TransferReturn.TransferRecord>(transferRecord, _currentUser.UserGuid), cancellationToken);

        return Result<Guid>.Success(transferRecord.Id);
    }
}