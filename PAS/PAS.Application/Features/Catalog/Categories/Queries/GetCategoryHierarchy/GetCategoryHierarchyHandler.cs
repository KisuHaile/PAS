using Application.Features.Catalog.Categories.Dtos;
using MediatR;

namespace Application.Features.Catalog.Categories.Queries.GetCategoryHierarchy;

public class GetCategoryHierarchyHandler : IRequestHandler<GetCategoryHierarchyQuery, Result<List<CategoryHierarchyDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetCategoryHierarchyHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<CategoryHierarchyDto>>> Handle(GetCategoryHierarchyQuery request, CancellationToken cancellationToken)
    {
        var categories = await _context.Categories
            .Where(c => !c.IsDeleted)
            .Include(c => c.SubCategories)
            .ToListAsync(cancellationToken);

        var rootCategories = categories.Where(c => c.ParentCategoryId == null).ToList();
        var hierarchy = rootCategories.Select(c => BuildHierarchy(c, categories)).ToList();

        return Result<List<CategoryHierarchyDto>>.Success(hierarchy);
    }

    private CategoryHierarchyDto BuildHierarchy(Category category, List<Category> allCategories)
    {
        var dto = new CategoryHierarchyDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            Children = new List<CategoryHierarchyDto>()
        };

        var children = allCategories.Where(c => c.ParentCategoryId == category.Id).ToList();
        foreach (var child in children)
        {
            dto.Children.Add(BuildHierarchy(child, allCategories));
        }

        return dto;
    }
}