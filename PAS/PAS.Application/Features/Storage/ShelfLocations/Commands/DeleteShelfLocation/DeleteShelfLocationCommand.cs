using Application.Common.Security;
using Application.Events;
using MediatR;

namespace Application.Features.Storage.ShelfLocations.Commands;

[Authorize(Permissions = Permissions.ShelfLocations.Delete)]
public record DeleteShelfLocationCommand(Guid Id) : IRequest<Result>;