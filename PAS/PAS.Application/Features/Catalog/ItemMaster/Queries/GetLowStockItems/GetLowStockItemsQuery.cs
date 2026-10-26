using Application.Common.Security;
using Application.Features.Catalog.ItemMasters.Dtos;
using MediatR;

namespace Application.Features.Catalog.ItemMasters.Queries.GetLowStockItems;

[Authorize(Permissions = Permissions.Items.ViewStock)]
public record GetLowStockItemsQuery : IRequest<Result<List<LowStockItemDto>>>;

public class LowStockItemDto
{
    public Guid ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public int CurrentStock { get; set; }
    public int MinStockLevel { get; set; }
    public int Deficit { get; set; }
    public List<string> Locations { get; set; } = new();
}