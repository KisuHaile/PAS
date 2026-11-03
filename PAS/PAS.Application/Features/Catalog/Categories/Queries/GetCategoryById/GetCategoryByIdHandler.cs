using Application.Features.Catalog.Categories.Dtos;
using AutoMapper;
using MediatR;

namespace Application.Features.Catalog.Categories.Queries.GetCategoryById;

public class GetCategoryByIdHandler : IRequestHandler<GetCategoryByIdQuery, Result<CategoryDetailDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetCategoryByIdHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<CategoryDetailDto>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .Include(c => c.SubCategories.Where(s => !s.IsDeleted))
            .Include(c => c.Items.Where(i => !i.IsDeleted))
                .ThenInclude(i => i.InventoryStocks)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.Id && !c.IsDeleted, cancellationToken);

        if (category == null)
        {
            throw new NotFoundException(nameof(Category), request.Id);
        }

        var categoryDto = _mapper.Map<CategoryDetailDto>(category);

        if (category.ParentCategoryId.HasValue)
        {
            var parent = await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == category.ParentCategoryId, cancellationToken);

            if (parent != null)
            {
                categoryDto.ParentCategoryName = parent.Name;
            }
        }

        categoryDto.SubCategories = category.SubCategories?
            .Select(s => new CategoryDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                ParentCategoryId = s.ParentCategoryId
            })
            .ToList() ?? new();

        categoryDto.Items = category.Items?
            .Select(i => new CategoryItemDto
            {
                Id = i.Id,
                SKU = i.SKU,
                ItemName = i.ItemName,
                UnitOfMeasure = i.UnitOfMeasure,
                CurrentStock = i.InventoryStocks?.Sum(s => s.CurrentQuantity) ?? 0
            })
            .ToList() ?? new();

        categoryDto.SubCategoriesCount = categoryDto.SubCategories.Count;
        categoryDto.ItemsCount = categoryDto.Items.Count;

        return Result<CategoryDetailDto>.Success(categoryDto);
    }
}