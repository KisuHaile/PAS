using Application.Features.PropertyManagement.Properties.Dtos;
using AutoMapper;
using MediatR;

namespace Application.Features.PropertyManagement.Properties.Queries.GetProperties;

public class GetPropertiesHandler : IRequestHandler<GetPropertiesQuery, Result<PaginatedList<PropertyDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetPropertiesHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedList<PropertyDto>>> Handle(GetPropertiesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Properties
            .Include(p => p.PropertyType)
            .Include(p => p.PropertyCategory)
            .Include(p => p.Location)
            .Include(p => p.SafetyBox)
            .Where(p => !p.IsDeleted)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(p =>
                p.TagNumber.Contains(request.SearchTerm) ||
                p.Name.Contains(request.SearchTerm) ||
                p.SerialNumber.Contains(request.SearchTerm));
        }

        if (request.LocationId.HasValue)
        {
            query = query.Where(p => p.LocationId == request.LocationId);
        }

        if (request.PropertyTypeId.HasValue)
        {
            query = query.Where(p => p.PropertyTypeId == request.PropertyTypeId);
        }

        if (request.PropertyCategoryId.HasValue)
        {
            query = query.Where(p => p.PropertyCategoryId == request.PropertyCategoryId);
        }

        if (request.SafetyBoxId.HasValue)
        {
            query = query.Where(p => p.SafetyBoxId == request.SafetyBoxId);
        }

        if (request.MinPrice.HasValue)
        {
            query = query.Where(p => p.UnitPrice >= request.MinPrice);
        }

        if (request.MaxPrice.HasValue)
        {
            query = query.Where(p => p.UnitPrice <= request.MaxPrice);
        }

        if (request.FromPurchaseDate.HasValue)
        {
            query = query.Where(p => p.PurchaseDate >= request.FromPurchaseDate);
        }

        if (request.ToPurchaseDate.HasValue)
        {
            query = query.Where(p => p.PurchaseDate <= request.ToPurchaseDate);
        }

        var paginatedProperties = await query
            .OrderBy(p => p.TagNumber)
            .ProjectTo<PropertyDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return Result<PaginatedList<PropertyDto>>.Success(paginatedProperties);
    }
}