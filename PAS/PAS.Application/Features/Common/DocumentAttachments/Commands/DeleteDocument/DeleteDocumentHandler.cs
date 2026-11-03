using Application.Events;
using MediatR;

namespace Application.Features.Common.DocumentAttachments.Commands.DeleteDocument;

public class DeleteDocumentCommandHandler : IRequestHandler<DeleteDocumentCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorageService _fileStorage;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public DeleteDocumentCommandHandler(
        IApplicationDbContext context,
        IFileStorageService fileStorage,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _fileStorage = fileStorage;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result> Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
    {
        var attachment = await _context.DocumentAttachments
            .FirstOrDefaultAsync(a => a.Id == request.Id && !a.IsDeleted, cancellationToken);

        if (attachment == null)
        {
            throw new NotFoundException(nameof(DocumentAttachment), request.Id);
        }

        // Delete from file storage
        await _fileStorage.DeleteFileAsync(attachment.FilePath, cancellationToken);

        // Soft delete record
        attachment.SoftDelete();

        // Create audit trail
        var auditTrail = new Domain.Common.AuditTrail(
            _currentUser.UserGuid ?? Guid.Empty,
            "DELETE_DOCUMENT",
            attachment.RelatedEntityName,
            attachment.RelatedEntityId);
        _context.AuditTrails.Add(auditTrail);

        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new EntityDeletedEvent<DocumentAttachment>(attachment, _currentUser.UserGuid), cancellationToken);

        return Result.Success();
    }
}