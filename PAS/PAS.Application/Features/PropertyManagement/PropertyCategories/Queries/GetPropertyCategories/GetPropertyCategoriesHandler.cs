using Application.Features.PropertyManagement.PropertyCategories.Dtos;
using AutoMapper;
using MediatR;

namespace Application.Features.PropertyManagement.PropertyCategories.Queries.GetPropertyCategories;

public class GetPropertyCategoriesHandler : IRequestHandler<GetPropertyCategoriesQuery, Result<List<PropertyCategoryDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetPropertyCategoriesHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<List<PropertyCategoryDto>>> Handle(GetPropertyCategoriesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.PropertyCategories
            .Include(c => c.Properties.Where(p => !p.IsDeleted))
            .Where(c => !c.IsDeleted)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(c => c.Name.Contains(request.SearchTerm));
        }

        var categories = await query
            .OrderBy(c => c.Name)
            .ProjectTo<PropertyCategoryDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        foreach (var category in categories)
        {
            var dbCategory = await _context.PropertyCategories
                .Include(c => c.Properties)
                .FirstOrDefaultAsync(c => c.Id == category.Id, cancellationToken);
            category.PropertiesCount = dbCategory?.Properties?.Count(p => !p.IsDeleted) ?? 0;
        }

        return Result<List<PropertyCategoryDto>>.Success(categories);
    }
}