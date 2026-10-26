using Application.Common.Security;
using Application.Features.Common.DocumentAttachments.Dtos;
using MediatR;

namespace Application.Features.Common.DocumentAttachments.Queries.GetDocumentById;

[Authorize(Permissions = Permissions.Documents.View)]
public record GetDocumentByIdQuery(Guid Id) : IRequest<Result<DocumentAttachmentDto>>;