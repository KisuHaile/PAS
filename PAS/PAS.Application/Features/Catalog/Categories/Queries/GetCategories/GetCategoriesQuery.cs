using Application.Common.Security;
using Application.Features.Catalog.Categories.Dtos;
using MediatR;

namespace Application.Features.Catalog.Categories.Queries.GetCategories;

[Authorize(Permissions = Permissions.Categories.View)]
public record GetCategoriesQuery : IRequest<Result<List<CategoryDto>>>
{
    public bool IncludeInactive { get; init; }
    public Guid? ParentCategoryId { get; init; }
    public string? SearchTerm { get; init; }
}