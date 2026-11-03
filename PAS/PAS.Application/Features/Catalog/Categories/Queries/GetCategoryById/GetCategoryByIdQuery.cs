using Application.Common.Security;
using Application.Features.Catalog.Categories.Dtos;
using MediatR;

namespace Application.Features.Catalog.Categories.Queries.GetCategoryById;

[Authorize(Permissions = Permissions.Categories.View)]
public record GetCategoryByIdQuery(Guid Id) : IRequest<Result<CategoryDetailDto>>;