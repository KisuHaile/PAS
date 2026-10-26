using Application.Common.Security;
using MediatR;

namespace Application.Features.PropertyManagement.Locations.Commands.DeleteLocation;

[Authorize(Permissions = Permissions.Locations.Delete)]
public record DeleteLocationCommand(Guid Id) : IRequest<Result>;