using Application.Common.Security;
using Application.Features.Receiving.ReceivingNotes.Dtos;
using MediatR;

namespace Application.Features.Receiving.ReceivingNotes.Queries;

[Authorize(Permissions = Permissions.Receiving.View)]
public record GetReceivingNotesQuery : IRequest<Result<PaginatedList<ReceivingNoteListDto>>>
{
    public string? Status { get; init; }
    public Guid? SupplierId { get; init; }
    public Guid? ReceivedById { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public string? SearchTerm { get; init; }
    public bool? HasInspection { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}