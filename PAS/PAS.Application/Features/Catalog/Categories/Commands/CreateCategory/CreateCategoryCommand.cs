using Application.Common.Security;
using MediatR;

namespace Application.Features.Catalog.Categories.Commands.CreateCategory;

[Authorize(Permissions = Permissions.Categories.Create)]
public record CreateCategoryCommand : IRequest<Result<Guid>>
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public Guid? ParentCategoryId { get; init; }
}