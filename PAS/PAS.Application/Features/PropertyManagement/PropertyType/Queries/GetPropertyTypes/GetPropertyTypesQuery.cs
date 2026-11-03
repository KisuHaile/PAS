using Application.Common.Security;
using Application.Features.PropertyManagement.PropertyTypes.Dtos;
using MediatR;

namespace Application.Features.PropertyManagement.PropertyTypes.Queries.GetPropertyTypes;

[Authorize(Permissions = Permissions.PropertyTypes.View)]
public record GetPropertyTypesQuery : IRequest<Result<List<PropertyTypeDto>>>
{
    public string? SearchTerm { get; init; }
}