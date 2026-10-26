using Application.Features.PropertyManagement.Properties.Dtos;
using AutoMapper;
using MediatR;

namespace Application.Features.PropertyManagement.Properties.Queries.GetPropertiesByLocation;

public class GetPropertiesByLocationHandler : IRequestHandler<GetPropertiesByLocationQuery, Result<List<PropertyDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetPropertiesByLocationHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<List<PropertyDto>>> Handle(GetPropertiesByLocationQuery request, CancellationToken cancellationToken)
    {
        var properties = await _context.Properties
            .Include(p => p.PropertyType)
            .Include(p => p.Location)
            .Include(p => p.SafetyBox)
            .Where(p => p.LocationId == request.LocationId && !p.IsDeleted)
            .OrderBy(p => p.TagNumber)
            .ProjectTo<PropertyDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return Result<List<PropertyDto>>.Success(properties);
    }
}