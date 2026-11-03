using Application.Features.Common.DocumentAttachments.Dtos;
using AutoMapper;
using MediatR;

namespace Application.Features.Common.DocumentAttachments.Queries.GetDocumentsByEntity;

public class GetDocumentsByEntityHandler : IRequestHandler<GetDocumentsByEntityQuery, Result<List<DocumentAttachmentDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IFileStorageService _fileStorage;

    public GetDocumentsByEntityHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IFileStorageService fileStorage)
    {
        _context = context;
        _mapper = mapper;
        _fileStorage = fileStorage;
    }

    public async Task<Result<List<DocumentAttachmentDto>>> Handle(GetDocumentsByEntityQuery request, CancellationToken cancellationToken)
    {
        var documents = await _context.DocumentAttachments
            .Where(d => d.RelatedEntityName == request.EntityName &&
                       d.RelatedEntityId == request.EntityId &&
                       !d.IsDeleted)
            .OrderByDescending(d => d.CreatedAt)
            .ProjectTo<DocumentAttachmentDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        // Add download URLs
        foreach (var doc in documents)
        {
            doc.DownloadUrl = await _fileStorage.GetFileUrlAsync(doc.FilePath);
        }

        return Result<List<DocumentAttachmentDto>>.Success(documents);
    }
}