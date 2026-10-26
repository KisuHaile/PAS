using Application.Common.Security;
using MediatR;

namespace Application.Features.PropertyManagement.Locations.Commands.CreateLocation;

[Authorize(Permissions = Permissions.Locations.Create)]
public record CreateLocationCommand : IRequest<Result<Guid>>
{
    public string Name { get; init; } = string.Empty;
    public string LocationType { get; init; } = string.Empty;
}