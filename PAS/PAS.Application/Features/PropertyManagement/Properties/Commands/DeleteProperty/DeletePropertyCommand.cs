using Application.Common.Security;
using MediatR;

namespace Application.Features.PropertyManagement.Properties.Commands.DeleteProperty;

[Authorize(Permissions = Permissions.Properties.Edit)]
public record DeletePropertyCommand(Guid Id) : IRequest<Result>;
