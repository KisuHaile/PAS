using Application.Features.PropertyManagement.Locations.Dtos;
using AutoMapper;
using MediatR;

namespace Application.Features.PropertyManagement.Locations.Queries.GetLocations;

public class GetLocationsHandler : IRequestHandler<GetLocationsQuery, Result<List<LocationDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetLocationsHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<List<LocationDto>>> Handle(GetLocationsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.PropertyLocations
            .Include(l => l.Properties.Where(p => !p.IsDeleted))
            .Include(l => l.SafetyBoxes.Where(s => !s.IsDeleted))
            .Where(l => !l.IsDeleted)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.LocationType))
        {
            query = query.Where(l => l.LocationType == request.LocationType);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(l => l.Name.Contains(request.SearchTerm));
        }

        var locations = await query
            .OrderBy(l => l.Name)
            .ProjectTo<LocationDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return Result<List<LocationDto>>.Success(locations);
    }
}