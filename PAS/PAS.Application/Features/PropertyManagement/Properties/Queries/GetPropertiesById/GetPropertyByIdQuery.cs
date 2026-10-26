using Application.Common.Security;
using Application.Features.PropertyManagement.Properties.Dtos;
using MediatR;

namespace Application.Features.PropertyManagement.Properties.Queries.GetPropertyById;

[Authorize(Permissions = Permissions.Properties.ViewDetails)]
public record GetPropertyByIdQuery(Guid Id) : IRequest<Result<PropertyDetailDto>>;