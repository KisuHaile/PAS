using Application.Common.Security;
using MediatR;

namespace Application.Features.PropertyManagement.PropertyCategories.Commands.CreatePropertyCategory;

[Authorize(Permissions = Permissions.PropertyCategories.Create)]
public record CreatePropertyCategoryCommand : IRequest<Result<Guid>>
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}