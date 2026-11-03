using Application.Common.Security;
using Application.Features.PropertyManagement.Properties.Dtos;
using MediatR;

namespace Application.Features.PropertyManagement.Properties.Queries.GetPropertiesByLocation;

[Authorize(Permissions = Permissions.Properties.View)]
public record GetPropertiesByLocationQuery(Guid LocationId) : IRequest<Result<List<PropertyDto>>>;