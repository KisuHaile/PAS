using Application.Common.Security;
using Application.Features.Common.DocumentAttachments.Dtos;
using MediatR;

namespace Application.Features.Common.DocumentAttachments.Queries.GetDocumentsByEntity;

[Authorize(Permissions = Permissions.Documents.View)]
public record GetDocumentsByEntityQuery : IRequest<Result<List<DocumentAttachmentDto>>>
{
    public string EntityName { get; init; } = string.Empty;
    public Guid EntityId { get; init; }
}