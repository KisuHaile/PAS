using Application.Features.PropertyManagement.Locations.Dtos;
using AutoMapper;
using MediatR;

namespace Application.Features.PropertyManagement.Locations.Queries.GetLocationById;

public class GetLocationByIdHandler : IRequestHandler<GetLocationByIdQuery, Result<LocationDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetLocationByIdHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<LocationDto>> Handle(GetLocationByIdQuery request, CancellationToken cancellationToken)
    {
        var location = await _context.PropertyLocations
            .Include(l => l.Properties.Where(p => !p.IsDeleted))
            .Include(l => l.SafetyBoxes.Where(s => !s.IsDeleted))
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == request.Id && !l.IsDeleted, cancellationToken);

        if (location == null)
        {
            throw new NotFoundException(nameof(PropertyLocation), request.Id);
        }

        var locationDto = _mapper.Map<LocationDto>(location);
        locationDto.PropertiesCount = location.Properties?.Count ?? 0;
        locationDto.SafetyBoxesCount = location.SafetyBoxes?.Count ?? 0;

        return Result<LocationDto>.Success(locationDto);
    }
}