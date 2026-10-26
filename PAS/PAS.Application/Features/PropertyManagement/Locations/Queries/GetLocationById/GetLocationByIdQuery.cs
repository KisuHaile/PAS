using Application.Common.Security;
using Application.Features.PropertyManagement.Locations.Dtos;
using MediatR;

namespace Application.Features.PropertyManagement.Locations.Queries.GetLocationById;

[Authorize(Permissions = Permissions.Locations.View)]
public record GetLocationByIdQuery(Guid Id) : IRequest<Result<LocationDto>>;