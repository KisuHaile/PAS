using Application.Features.PropertyManagement.Properties.Dtos;
using AutoMapper;
using MediatR;

namespace Application.Features.PropertyManagement.Properties.Queries.GetPropertyById;

public class GetPropertyByIdHandler : IRequestHandler<GetPropertyByIdQuery, Result<PropertyDetailDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetPropertyByIdHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<PropertyDetailDto>> Handle(GetPropertyByIdQuery request, CancellationToken cancellationToken)
    {
        var property = await _context.Properties
            .Include(p => p.PropertyType)
            .Include(p => p.PropertyCategory)
            .Include(p => p.Location)
            .Include(p => p.SafetyBox)
            .Include(p => p.Attachments.Where(a => !a.IsDeleted))
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.Id && !p.IsDeleted, cancellationToken);

        if (property == null)
        {
            throw new NotFoundException(nameof(Domain.PropertyManagement.Property), request.Id);
        }

        var propertyDto = _mapper.Map<PropertyDetailDto>(property);

        propertyDto.Attachments = property.Attachments?
            .Select(a => new PropertyAttachmentDto
            {
                Id = a.Id,
                FileName = a.FileName,
                ContentType = a.ContentType,
                FileSize = 0,
                UploadedAt = a.CreatedAt
            })
            .ToList() ?? new();

        var movements = await _context.TransferRecords
            .Include(t => t.FromLocation)
            .Include(t => t.ToLocation)
            .Where(t => t.ItemId == request.Id)
            .OrderByDescending(t => t.TransferDate)
            .Select(t => new PropertyMovementDto
            {
                Date = t.TransferDate,
                TransactionType = "TRANSFER",
                Reference = t.Id.ToString(),
                FromLocation = t.FromLocation != null ? t.FromLocation.Name : "Unknown",
                ToLocation = t.ToLocation != null ? t.ToLocation.Name : "Unknown",
                PerformedBy = "System",
                Remarks = $"Quantity: {t.Quantity}"
            })
            .ToListAsync(cancellationToken);

        propertyDto.MovementHistory = movements;

        return Result<PropertyDetailDto>.Success(propertyDto);
    }
}