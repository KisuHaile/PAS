using Application.Common.Security;
using MediatR;

namespace Application.Features.Catalog.Categories.Commands.UpdateCategory;

[Authorize(Permissions = Permissions.Categories.Edit)]
public record UpdateCategoryCommand : IRequest<Result>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public Guid? ParentCategoryId { get; init; }
}