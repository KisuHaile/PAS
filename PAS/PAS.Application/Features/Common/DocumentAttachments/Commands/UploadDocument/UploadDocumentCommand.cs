using Application.Common.Security;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Common.DocumentAttachments.Commands.UploadDocument;

[Authorize(Permissions = Permissions.Documents.Upload)]
public record UploadDocumentCommand : IRequest<Result<Guid>>
{
    public IFormFile File { get; init; } = null!;
    public Guid RelatedEntityId { get; init; }
    public string RelatedEntityName { get; init; } = string.Empty;
}