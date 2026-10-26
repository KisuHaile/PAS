using Application.Common.Security;
using MediatR;

namespace Application.Features.PropertyManagement.Locations.Commands.UpdateLocation;

[Authorize(Permissions = Permissions.Locations.Edit)]
public record UpdateLocationCommand : IRequest<Result>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string LocationType { get; init; } = string.Empty;
}