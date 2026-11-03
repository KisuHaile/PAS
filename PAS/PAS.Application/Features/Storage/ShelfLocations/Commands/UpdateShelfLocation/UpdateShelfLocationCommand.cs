using Application.Common.Security;
using Application.Events;
using MediatR;

namespace Application.Features.Storage.ShelfLocations.Commands;

[Authorize(Permissions = Permissions.ShelfLocations.Edit)]
public record UpdateShelfLocationCommand : IRequest<Result>
{
    public Guid Id { get; init; }
    public string Aisle { get; init; } = string.Empty;
    public string Rack { get; init; } = string.Empty;
    public string ShelfNumber { get; init; } = string.Empty;
    public string Zone { get; init; } = string.Empty;
    public string BinType { get; init; } = string.Empty;
    public decimal? Length { get; init; }
    public decimal? Width { get; init; }
    public decimal? Height { get; init; }
    public decimal? MaxWeight { get; init; }
    public int Capacity { get; init; }
    public bool IsActive { get; init; }
}