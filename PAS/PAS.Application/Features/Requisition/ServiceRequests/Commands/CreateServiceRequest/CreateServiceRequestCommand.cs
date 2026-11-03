using Application.Common.Security;
using Application.Events;
using Application.Features.Requisition.ServiceRequests.Dtos;
using MediatR;

namespace Application.Features.Requisition.ServiceRequests.Commands;

[Authorize(Permissions = Permissions.Requisitions.Create)]
public record CreateServiceRequestCommand : IRequest<Result<Guid>>
{
    public List<ServiceRequestItemDto> Items { get; init; } = new();
    public string? Remarks { get; init; }
}