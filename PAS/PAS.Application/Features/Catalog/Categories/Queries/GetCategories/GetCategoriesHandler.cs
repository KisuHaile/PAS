using Application.Features.Catalog.Categories.Dtos;
using AutoMapper;
using MediatR;

namespace Application.Features.Catalog.Categories.Queries.GetCategories;

public class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, Result<List<CategoryDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetCategoriesHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<List<CategoryDto>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Categories
            .Include(c => c.SubCategories)
            .Include(c => c.Items)
            .AsNoTracking();

        if (!request.IncludeInactive)
        {
            query = query.Where(c => !c.IsDeleted);
        }

        if (request.ParentCategoryId.HasValue)
        {
            query = query.Where(c => c.ParentCategoryId == request.ParentCategoryId);
        }
        else
        {
            query = query.Where(c => c.ParentCategoryId == null);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(c => c.Name.Contains(request.SearchTerm) ||
                                     c.Description.Contains(request.SearchTerm));
        }

        var categories = await query
            .OrderBy(c => c.Name)
            .ProjectTo<CategoryDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        foreach (var category in categories)
        {
            var dbCategory = await _context.Categories
                .Include(c => c.SubCategories)
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == category.Id, cancellationToken);

            category.SubCategoriesCount = dbCategory?.SubCategories?.Count(s => !s.IsDeleted) ?? 0;
            category.ItemsCount = dbCategory?.Items?.Count(i => !i.IsDeleted) ?? 0;

            if (category.ParentCategoryId.HasValue)
            {
                var parent = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Id == category.ParentCategoryId, cancellationToken);
                category.ParentCategoryName = parent?.Name;
            }
        }

        return Result<List<CategoryDto>>.Success(categories);
    }
}