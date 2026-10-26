using Application.Common.Security;
using Application.Events;
using MediatR;

namespace Application.Features.Receiving.ReceivingNotes.Commands;

[Authorize(Permissions = Permissions.Receiving.Inspect)]
public record ApproveReceivingNoteCommand : IRequest<Result>
{
    public Guid Id { get; init; }
    public bool IsApproved { get; init; }
    public string Remarks { get; init; } = string.Empty;
}