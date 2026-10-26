using Application.Features.PropertyManagement.SafetyBoxes.Dtos;
using AutoMapper;
using MediatR;

namespace Application.Features.PropertyManagement.SafetyBoxes.Queries.GetSafetyBoxById;

public class GetSafetyBoxByIdHandler : IRequestHandler<GetSafetyBoxByIdQuery, Result<SafetyBoxDetailDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetSafetyBoxByIdHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<SafetyBoxDetailDto>> Handle(GetSafetyBoxByIdQuery request, CancellationToken cancellationToken)
    {
        var safetyBox = await _context.SafetyBoxes
            .Include(s => s.Location)
            .Include(s => s.Shelves)
                .ThenInclude(sh => sh.Properties.Where(p => !p.IsDeleted))
            .Include(s => s.Properties)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.Id && !s.IsDeleted, cancellationToken);

        if (safetyBox == null)
        {
            throw new NotFoundException(nameof(SafetyBox), request.Id);
        }

        var safetyBoxDto = _mapper.Map<SafetyBoxDetailDto>(safetyBox);

        safetyBoxDto.OccupiedShelves = safetyBox.Shelves?.Count(sh => sh.Properties?.Any() == true) ?? 0;
        safetyBoxDto.PropertiesCount = safetyBox.Properties?.Count ?? 0;

        safetyBoxDto.Shelves = safetyBox.Shelves?
            .OrderBy(sh => sh.ShelfNumber)
            .Select(sh => new SafetyBoxShelfDto
            {
                Id = sh.Id,
                ShelfNumber = sh.ShelfNumber,
                PropertiesCount = sh.Properties?.Count ?? 0
            })
            .ToList() ?? new();

        safetyBoxDto.Properties = safetyBox.Properties?
            .Select(p => new SafetyBoxPropertyDto
            {
                Id = p.Id,
                TagNumber = p.TagNumber,
                Name = p.Name,
                ShelfNumber = 0 // Would need to track which shelf
            })
            .ToList() ?? new();

        return Result<SafetyBoxDetailDto>.Success(safetyBoxDto);
    }
}