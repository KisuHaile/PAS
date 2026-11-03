using Application.Common.Security;
using Application.Events;
using MediatR;

namespace Application.Features.Storage.InventoryStock.Commands;

[Authorize(Permissions = Permissions.Inventory.Adjust)]
public record AdjustStockCommand : IRequest<Result>
{
    public Guid InventoryId { get; init; }
    public int NewQuantity { get; init; }
    public string Reason { get; init; } = string.Empty;
    public string? Remarks { get; init; }
}