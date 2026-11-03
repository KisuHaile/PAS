using Application.Common.Security;
using Application.Features.Catalog.ItemMasters.Dtos;
using MediatR;

namespace Application.Features.Catalog.ItemMasters.Queries.GetItemMasters;

[Authorize(Permissions = Permissions.Items.View)]
public record GetItemMastersQuery : IRequest<Result<PaginatedList<ItemMasterListDto>>>
{
    public string? SearchTerm { get; init; }
    public Guid? CategoryId { get; init; }
    public bool? LowStockOnly { get; init; }
    public bool? RequiresInspection { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}