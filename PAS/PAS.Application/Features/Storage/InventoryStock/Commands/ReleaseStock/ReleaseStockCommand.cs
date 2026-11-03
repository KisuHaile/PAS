using Application.Common.Security;
using Application.Events;
using MediatR;

namespace Application.Features.Storage.InventoryStock.Commands;

[Authorize(Permissions = Permissions.Inventory.Release)]
public record ReleaseStockCommand : IRequest<Result>
{
    public Guid ItemId { get; init; }
    public int Quantity { get; init; }
    public Guid? ShelfId { get; init; }
    public Guid ReferenceId { get; init; }
    public string ReferenceType { get; init; } = string.Empty;
}