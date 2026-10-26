using Application.Common.Interfaces;
using Application.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Common.DocumentAttachments.Commands.UploadDocument;

public class UploadDocumentCommandHandler : IRequestHandler<UploadDocumentCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorageService _fileStorage;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;
    private readonly IImageAnalysisService _imageAnalysis;

    public UploadDocumentCommandHandler(
        IApplicationDbContext context,
        IFileStorageService fileStorage,
        ICurrentUserService currentUser,
        IMediator mediator,
        IImageAnalysisService imageAnalysis)
    {
        _context = context;
        _fileStorage = fileStorage;
        _currentUser = currentUser;
        _mediator = mediator;
        _imageAnalysis = imageAnalysis;
    }

    public async Task<Result<Guid>> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
    {
        // Validate related entity exists
        var entityExists = await ValidateRelatedEntityExists(request.RelatedEntityName, request.RelatedEntityId, cancellationToken);
        if (!entityExists)
        {
            return Result<Guid>.Failure($"Related entity {request.RelatedEntityName} with ID {request.RelatedEntityId} not found.");
        }

        // Save file to storage
        byte[] fileBytes;
        using (var memoryStream = new MemoryStream())
        {
            await request.File.CopyToAsync(memoryStream, cancellationToken);
            fileBytes = memoryStream.ToArray();
        }

        if (request.RelatedEntityName.Equals("employee", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                var containsPerson = await _imageAnalysis.ContainsPersonAsync(fileBytes, cancellationToken);
                if (!containsPerson)
                {
                    return Result<Guid>.Failure("Employee photo must contain a person.");
                }
            }
            catch (InvalidOperationException)
            {
                return Result<Guid>.Failure("Person detection is not configured.");
            }
        }

        var folder = $"documents/{request.RelatedEntityName}";
        var filePath = await _fileStorage.SaveFileAsync(fileBytes, request.File.FileName, folder, cancellationToken);

        // Create document attachment record
        var attachment = new DocumentAttachment(
            request.File.FileName,
            filePath,
            request.File.ContentType,
            request.RelatedEntityId,
            request.RelatedEntityName);

        _context.DocumentAttachments.Add(attachment);
        await _context.SaveChangesAsync(cancellationToken);

        // Create audit trail
        var auditTrail = new Domain.Common.AuditTrail(
            _currentUser.UserGuid ?? Guid.Empty,
            "UPLOAD_DOCUMENT",
            request.RelatedEntityName,
            request.RelatedEntityId);
        _context.AuditTrails.Add(auditTrail);
        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new EntityCreatedEvent<DocumentAttachment>(attachment, _currentUser.UserGuid), cancellationToken);

        return Result<Guid>.Success(attachment.Id);
    }

    private async Task<bool> ValidateRelatedEntityExists(string entityName, Guid entityId, CancellationToken cancellationToken)
    {
        return entityName.ToLower() switch
        {
            "property" => await _context.Properties.AnyAsync(p => p.Id == entityId && !p.IsDeleted, cancellationToken),
            "itemmaster" => await _context.ItemMasters.AnyAsync(i => i.Id == entityId && !i.IsDeleted, cancellationToken),
            "servicerequest" => await _context.ServiceRequests.AnyAsync(s => s.Id == entityId && !s.IsDeleted, cancellationToken),
            "receivingnote" => await _context.ReceivingNotes.AnyAsync(r => r.Id == entityId && !r.IsDeleted, cancellationToken),
            "supplier" => await _context.Suppliers.AnyAsync(s => s.Id == entityId && !s.IsDeleted, cancellationToken),
            "employee" => await _context.Employees.AnyAsync(e => e.Id == entityId && !e.IsDeleted, cancellationToken),
            _ => true // Skip validation for unknown entity types
        };
    }
}