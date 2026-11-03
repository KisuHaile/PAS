using Application.Features.PropertyManagement.SafetyBoxes.Dtos;
using AutoMapper;
using MediatR;

namespace Application.Features.PropertyManagement.SafetyBoxes.Queries.GetSafetyBoxes;

public class GetSafetyBoxesHandler : IRequestHandler<GetSafetyBoxesQuery, Result<List<SafetyBoxDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetSafetyBoxesHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<List<SafetyBoxDto>>> Handle(GetSafetyBoxesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.SafetyBoxes
            .Include(s => s.Location)
            .Include(s => s.Shelves)
            .Include(s => s.Properties)
            .Where(s => !s.IsDeleted)
            .AsNoTracking();

        if (request.LocationId.HasValue)
        {
            query = query.Where(s => s.LocationId == request.LocationId);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(s => s.BoxNumber.Contains(request.SearchTerm));
        }

        var safetyBoxes = await query
            .OrderBy(s => s.BoxNumber)
            .ProjectTo<SafetyBoxDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return Result<List<SafetyBoxDto>>.Success(safetyBoxes);
    }
}