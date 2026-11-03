using Application.Common.Security;
using Application.Features.PropertyManagement.Properties.Dtos;
using MediatR;

namespace Application.Features.PropertyManagement.Properties.Queries.GetProperties;

[Authorize(Permissions = Permissions.Properties.View)]
public record GetPropertiesQuery : IRequest<Result<PaginatedList<PropertyDto>>>
{
    public string? SearchTerm { get; init; }
    public Guid? LocationId { get; init; }
    public Guid? PropertyTypeId { get; init; }
    public Guid? PropertyCategoryId { get; init; }
    public Guid? SafetyBoxId { get; init; }
    public decimal? MinPrice { get; init; }
    public decimal? MaxPrice { get; init; }
    public DateTime? FromPurchaseDate { get; init; }
    public DateTime? ToPurchaseDate { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}