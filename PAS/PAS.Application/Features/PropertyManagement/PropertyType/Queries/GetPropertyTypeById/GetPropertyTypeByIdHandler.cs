using Application.Features.PropertyManagement.PropertyTypes.Dtos;
using AutoMapper;
using MediatR;

namespace Application.Features.PropertyManagement.PropertyTypes.Queries.GetPropertyTypeById;

public class GetPropertyTypeByIdHandler : IRequestHandler<GetPropertyTypeByIdQuery, Result<PropertyTypeDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetPropertyTypeByIdHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<PropertyTypeDto>> Handle(GetPropertyTypeByIdQuery request, CancellationToken cancellationToken)
    {
        var propertyType = await _context.PropertyTypes
            .Include(p => p.Properties.Where(pr => !pr.IsDeleted))
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.Id && !p.IsDeleted, cancellationToken);

        if (propertyType == null)
        {
            throw new NotFoundException(nameof(PropertyType), request.Id);
        }

        var propertyTypeDto = _mapper.Map<PropertyTypeDto>(propertyType);
        propertyTypeDto.PropertiesCount = propertyType.Properties?.Count ?? 0;

        return Result<PropertyTypeDto>.Success(propertyTypeDto);
    }
}