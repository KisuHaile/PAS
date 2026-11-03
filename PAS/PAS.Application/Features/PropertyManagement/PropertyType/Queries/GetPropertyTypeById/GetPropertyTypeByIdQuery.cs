using Application.Common.Security;
using Application.Features.PropertyManagement.PropertyTypes.Dtos;
using MediatR;

namespace Application.Features.PropertyManagement.PropertyTypes.Queries.GetPropertyTypeById;

[Authorize(Permissions = Permissions.PropertyTypes.View)]
public record GetPropertyTypeByIdQuery(Guid Id) : IRequest<Result<PropertyTypeDto>>;