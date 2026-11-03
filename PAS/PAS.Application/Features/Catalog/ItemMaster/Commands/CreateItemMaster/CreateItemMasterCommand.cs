using Application.Common.Security;
using MediatR;

namespace Application.Features.Catalog.ItemMasters.Commands.CreateItemMaster;

[Authorize(Permissions = Permissions.Items.Create)]
public record CreateItemMasterCommand : IRequest<Result<Guid>>
{
    public string SKU { get; init; } = string.Empty;
    public string ItemName { get; init; } = string.Empty;
    public Guid CategoryId { get; init; }
    public string UnitOfMeasure { get; init; } = string.Empty;
    public bool RequiresInspection { get; init; }
    public int MinStockLevel { get; init; }
    public decimal UnitPrice { get; init; }
    public string Status { get; init; } = "Active";
}