using Application.Common.Security;
using Application.Events;
using MediatR;

namespace Application.Features.Requisition.ServiceRequests.Commands;

[Authorize(Permissions = Permissions.Requisitions.Approve)]
public record ApproveServiceRequestCommand : IRequest<Result>
{
    public Guid Id { get; init; }
    public bool IsApproved { get; init; }
    public string? Remarks { get; init; }
}