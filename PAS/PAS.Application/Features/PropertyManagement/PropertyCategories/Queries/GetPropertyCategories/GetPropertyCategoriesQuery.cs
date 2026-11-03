using Application.Common.Security;
using Application.Features.PropertyManagement.PropertyCategories.Dtos;
using MediatR;

namespace Application.Features.PropertyManagement.PropertyCategories.Queries.GetPropertyCategories;

[Authorize(Permissions = Permissions.PropertyCategories.View)]
public record GetPropertyCategoriesQuery : IRequest<Result<List<PropertyCategoryDto>>>
{
    public string? SearchTerm { get; init; }
}