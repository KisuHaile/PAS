using Application.Features.PropertyManagement.PropertyTypes.Dtos;
using AutoMapper;
using MediatR;

namespace Application.Features.PropertyManagement.PropertyTypes.Queries.GetPropertyTypes;

public class GetPropertyTypesHandler : IRequestHandler<GetPropertyTypesQuery, Result<List<PropertyTypeDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetPropertyTypesHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<List<PropertyTypeDto>>> Handle(GetPropertyTypesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.PropertyTypes
            .Include(p => p.Properties.Where(pr => !pr.IsDeleted))
            .Where(p => !p.IsDeleted)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(p => p.Name.Contains(request.SearchTerm));
        }

        var propertyTypes = await query
            .OrderBy(p => p.Name)
            .ProjectTo<PropertyTypeDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return Result<List<PropertyTypeDto>>.Success(propertyTypes);
    }
}