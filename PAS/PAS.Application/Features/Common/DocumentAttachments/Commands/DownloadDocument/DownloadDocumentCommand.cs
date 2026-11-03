using Application.Common.Security;
using Application.Features.Common.DocumentAttachments.Dtos;
using MediatR;

namespace Application.Features.Common.DocumentAttachments.Commands.DownloadDocument;

[Authorize(Permissions = Permissions.Documents.Download)]
public record DownloadDocumentCommand(Guid Id) : IRequest<Result<FileDto>>;