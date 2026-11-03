using Application.Common.Security;
using Application.Features.PropertyManagement.Locations.Dtos;
using MediatR;

namespace Application.Features.PropertyManagement.Locations.Queries.GetLocations;

[Authorize(Permissions = Permissions.Locations.View)]
public record GetLocationsQuery : IRequest<Result<List<LocationDto>>>
{
    public string? LocationType { get; init; }
    public string? SearchTerm { get; init; }
}