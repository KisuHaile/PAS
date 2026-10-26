using Application.Common.Security;
using Application.Events;
using MediatR;

namespace Application.Features.TransferReturn.TransferRecords.Commands;

[Authorize(Permissions = Permissions.TransferReturn.Approve)]
public record ApproveTransferCommand : IRequest<Result>
{
    public Guid Id { get; init; }
    public bool IsApproved { get; init; }
    public string? Remarks { get; init; }
}