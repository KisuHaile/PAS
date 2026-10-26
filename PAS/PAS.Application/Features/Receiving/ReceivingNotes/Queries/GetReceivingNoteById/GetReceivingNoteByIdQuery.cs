using Application.Common.Security;
using Application.Features.Receiving.ReceivingNotes.Dtos;
using MediatR;

namespace Application.Features.Receiving.ReceivingNotes.Queries;

[Authorize(Permissions = Permissions.Receiving.View)]
public record GetReceivingNoteByIdQuery(Guid Id) : IRequest<Result<ReceivingNoteDetailDto>>;