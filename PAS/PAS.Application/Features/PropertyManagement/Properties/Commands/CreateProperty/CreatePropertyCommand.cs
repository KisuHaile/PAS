using Application.Common.Security;
using MediatR;

namespace Application.Features.PropertyManagement.Properties.Commands.CreateProperty;

[Authorize(Permissions = Permissions.Properties.Create)]
public record CreatePropertyCommand : IRequest<Result<Guid>>
{
    public string TagNumber { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string SerialNumber { get; init; } = string.Empty;
    public Guid PropertyTypeId { get; init; }
    public Guid? PropertyCategoryId { get; init; }
    public decimal UnitPrice { get; init; }
    public int Quantity { get; init; }
    public DateTime PurchaseDate { get; init; }
    public Guid LocationId { get; init; }
    public Guid? SafetyBoxId { get; init; }
    public int? ShelfNumber { get; init; }
}