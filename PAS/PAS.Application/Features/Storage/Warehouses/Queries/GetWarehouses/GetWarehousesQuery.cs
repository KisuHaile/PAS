using Application.Common.Security;
using Application.Features.Storage.Warehouses.Dtos;
using MediatR;

namespace Application.Features.Storage.Warehouses.Queries;

[Authorize(Permissions = Permissions.Warehouses.View)]
public record GetWarehousesQuery : IRequest<Result<PaginatedList<WarehouseListDto>>>
{
    public string? SearchTerm { get; init; }
    public bool? IsActive { get; init; }
    public string? City { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}