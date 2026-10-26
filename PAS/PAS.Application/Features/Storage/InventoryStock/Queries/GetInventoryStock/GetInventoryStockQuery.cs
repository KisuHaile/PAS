using Application.Common.Security;
using Application.Features.Storage.InventoryStock.Dtos;
using MediatR;

namespace Application.Features.Storage.InventoryStock.Queries;

[Authorize(Permissions = Permissions.Inventory.View)]
public record GetInventoryStockQuery : IRequest<Result<PaginatedList<InventoryStockDto>>>
{
    public Guid? ItemId { get; init; }
    public Guid? ShelfId { get; init; }
    public Guid? WarehouseId { get; init; }
    public bool? LowStockOnly { get; init; }
    public bool? ExpiringSoon { get; init; }
    public int? ExpiryDays { get; init; }
    public string? BatchNumber { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}