using Application.Features.Common.DocumentAttachments.Dtos;
using AutoMapper;
using MediatR;

namespace Application.Features.Common.DocumentAttachments.Queries.GetDocumentById;

public class GetDocumentByIdHandler : IRequestHandler<GetDocumentByIdQuery, Result<DocumentAttachmentDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IFileStorageService _fileStorage;

    public GetDocumentByIdHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IFileStorageService fileStorage)
    {
        _context = context;
        _mapper = mapper;
        _fileStorage = fileStorage;
    }

    public async Task<Result<DocumentAttachmentDto>> Handle(GetDocumentByIdQuery request, CancellationToken cancellationToken)
    {
        var document = await _context.DocumentAttachments
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == request.Id && !d.IsDeleted, cancellationToken);

        if (document == null)
        {
            throw new NotFoundException(nameof(DocumentAttachment), request.Id);
        }

        var documentDto = _mapper.Map<DocumentAttachmentDto>(document);
        documentDto.DownloadUrl = await _fileStorage.GetFileUrlAsync(document.FilePath);

        return Result<DocumentAttachmentDto>.Success(documentDto);
    }
}