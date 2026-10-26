using Application.Common.Security;
using Application.Features.Storage.ShelfLocations.Dtos;
using MediatR;

namespace Application.Features.Storage.ShelfLocations.Queries;

[Authorize(Permissions = Permissions.ShelfLocations.View)]
public record GetShelfLocationsQuery : IRequest<Result<PaginatedList<ShelfLocationListDto>>>
{
    public Guid? WarehouseId { get; init; }
    public string? Zone { get; init; }
    public string? BinType { get; init; }
    public bool? IsActive { get; init; }
    public bool? HasInventory { get; init; }
    public string? SearchTerm { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}