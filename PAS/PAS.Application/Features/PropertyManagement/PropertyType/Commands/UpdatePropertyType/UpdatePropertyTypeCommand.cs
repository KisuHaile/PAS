using Application.Common.Security;
using MediatR;

namespace Application.Features.PropertyManagement.PropertyTypes.Commands.UpdatePropertyType;

[Authorize(Permissions = Permissions.PropertyTypes.Edit)]
public record UpdatePropertyTypeCommand : IRequest<Result>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}