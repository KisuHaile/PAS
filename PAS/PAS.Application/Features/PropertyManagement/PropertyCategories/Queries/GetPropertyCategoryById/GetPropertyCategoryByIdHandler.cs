using Application.Features.PropertyManagement.PropertyCategories.Dtos;
using AutoMapper;
using MediatR;

namespace Application.Features.PropertyManagement.PropertyCategories.Queries.GetPropertyCategoryById;

public class GetPropertyCategoryByIdHandler : IRequestHandler<GetPropertyCategoryByIdQuery, Result<PropertyCategoryDetailDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetPropertyCategoryByIdHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<PropertyCategoryDetailDto>> Handle(GetPropertyCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _context.PropertyCategories
            .Include(c => c.Properties.Where(p => !p.IsDeleted))
                .ThenInclude(p => p.Location)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.Id && !c.IsDeleted, cancellationToken);

        if (category == null)
        {
            throw new NotFoundException(nameof(PropertyCategory), request.Id);
        }

        var categoryDto = _mapper.Map<PropertyCategoryDetailDto>(category);
        categoryDto.PropertiesCount = category.Properties?.Count ?? 0;

        categoryDto.Properties = category.Properties?
            .Select(p => new PropertyCategoryPropertyDto
            {
                Id = p.Id,
                TagNumber = p.TagNumber,
                Name = p.Name,
                LocationName = p.Location?.Name ?? "Unknown"
            })
            .ToList() ?? new();

        return Result<PropertyCategoryDetailDto>.Success(categoryDto);
    }
}