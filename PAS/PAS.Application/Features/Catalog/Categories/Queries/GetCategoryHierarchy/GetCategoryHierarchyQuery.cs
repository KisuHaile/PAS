using Application.Common.Security;
using Application.Features.Catalog.Categories.Dtos;
using MediatR;

namespace Application.Features.Catalog.Categories.Queries.GetCategoryHierarchy;

[Authorize(Permissions = Permissions.Categories.View)]
public record GetCategoryHierarchyQuery : IRequest<Result<List<CategoryHierarchyDto>>>;