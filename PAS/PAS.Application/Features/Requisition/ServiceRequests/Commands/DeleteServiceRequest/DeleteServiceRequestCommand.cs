using Application.Common.Security;
using Application.Events;
using MediatR;

namespace Application.Features.Requisition.ServiceRequests.Commands;

[Authorize(Permissions = Permissions.Requisitions.Delete)]
public record DeleteServiceRequestCommand(Guid Id) : IRequest<Result>;