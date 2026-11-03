using Application.Common.Security;
using MediatR;

namespace Application.Features.Common.DocumentAttachments.Commands.DeleteDocument;

[Authorize(Permissions = Permissions.Documents.Delete)]
public record DeleteDocumentCommand(Guid Id) : IRequest<Result>;