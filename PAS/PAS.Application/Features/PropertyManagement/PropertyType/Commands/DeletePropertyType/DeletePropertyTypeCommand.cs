using Application.Common.Security;
using MediatR;

namespace Application.Features.PropertyManagement.PropertyTypes.Commands.DeletePropertyType;

[Authorize(Permissions = Permissions.PropertyTypes.Delete)]
public record DeletePropertyTypeCommand(Guid Id) : IRequest<Result>;