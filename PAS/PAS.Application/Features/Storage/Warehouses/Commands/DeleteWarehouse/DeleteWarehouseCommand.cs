using Application.Common.Security;
using Application.Events;
using MediatR;

namespace Application.Features.Storage.Warehouses.Commands;

[Authorize(Permissions = Permissions.Warehouses.Delete)]
public record DeleteWarehouseCommand(Guid Id) : IRequest<Result>;