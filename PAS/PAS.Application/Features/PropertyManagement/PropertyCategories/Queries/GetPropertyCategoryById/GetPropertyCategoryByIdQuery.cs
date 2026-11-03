using Application.Common.Security;
using Application.Features.PropertyManagement.PropertyCategories.Dtos;
using MediatR;

namespace Application.Features.PropertyManagement.PropertyCategories.Queries.GetPropertyCategoryById;

[Authorize(Permissions = Permissions.PropertyCategories.View)]
public record GetPropertyCategoryByIdQuery(Guid Id) : IRequest<Result<PropertyCategoryDetailDto>>;