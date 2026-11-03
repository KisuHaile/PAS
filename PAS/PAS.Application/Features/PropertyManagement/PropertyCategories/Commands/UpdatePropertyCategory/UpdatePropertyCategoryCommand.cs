using Application.Common.Security;
using MediatR;

namespace Application.Features.PropertyManagement.PropertyCategories.Commands.UpdatePropertyCategory;

[Authorize(Permissions = Permissions.PropertyCategories.Edit)]
public record UpdatePropertyCategoryCommand : IRequest<Result>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}