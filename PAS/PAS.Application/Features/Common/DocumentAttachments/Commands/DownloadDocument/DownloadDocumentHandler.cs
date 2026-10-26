using Application.Features.Common.DocumentAttachments.Dtos;
using MediatR;

namespace Application.Features.Common.DocumentAttachments.Commands.DownloadDocument;

public class DownloadDocumentCommandHandler : IRequestHandler<DownloadDocumentCommand, Result<FileDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorageService _fileStorage;
    private readonly ICurrentUserService _currentUser;

    public DownloadDocumentCommandHandler(
        IApplicationDbContext context,
        IFileStorageService fileStorage,
        ICurrentUserService currentUser)
    {
        _context = context;
        _fileStorage = fileStorage;
        _currentUser = currentUser;
    }

    public async Task<Result<FileDto>> Handle(DownloadDocumentCommand request, CancellationToken cancellationToken)
    {
        var attachment = await _context.DocumentAttachments
            .FirstOrDefaultAsync(a => a.Id == request.Id && !a.IsDeleted, cancellationToken);

        if (attachment == null)
        {
            throw new NotFoundException(nameof(DocumentAttachment), request.Id);
        }

        var fileContent = await _fileStorage.GetFileAsync(attachment.FilePath, cancellationToken);

        var auditTrail = new Domain.Common.AuditTrail(
            _currentUser.UserGuid ?? Guid.Empty,
            "DOWNLOAD_DOCUMENT",
            attachment.RelatedEntityName,
            attachment.RelatedEntityId);
        _context.AuditTrails.Add(auditTrail);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<FileDto>.Success(new FileDto
        {
            FileName = attachment.FileName,
            Content = fileContent,
            ContentType = attachment.ContentType,
            Size = fileContent.Length
        });
    }
}