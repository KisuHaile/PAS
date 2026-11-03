using Application.Common.Security;
using MediatR;

namespace Application.Features.PropertyManagement.PropertyTypes.Commands.CreatePropertyType;

[Authorize(Permissions = Permissions.PropertyTypes.Create)]
public record CreatePropertyTypeCommand : IRequest<Result<Guid>>
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}