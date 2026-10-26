using Application.Features.Common.AuditTrail.Dtos;
using AutoMapper;
using MediatR;

namespace Application.Features.Common.AuditTrail.Queries.GetAuditTrails;

public class GetAuditTrailsHandler : IRequestHandler<GetAuditTrailsQuery, Result<PaginatedList<AuditTrailListDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAuditTrailsHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedList<AuditTrailListDto>>> Handle(GetAuditTrailsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.AuditTrails
            .Where(a => !a.IsDeleted)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.UserName))
        {
            query = query.Where(a => false);
        }

        if (!string.IsNullOrWhiteSpace(request.Action))
        {
            query = query.Where(a => a.Action.Contains(request.Action));
        }

        if (!string.IsNullOrWhiteSpace(request.EntityName))
        {
            query = query.Where(a => a.EntityName.Contains(request.EntityName));
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(a => a.ActionDate >= request.FromDate);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(a => a.ActionDate <= request.ToDate);
        }

        var paginatedTrails = await query
            .OrderByDescending(a => a.ActionDate)
            .ProjectTo<AuditTrailListDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return Result<PaginatedList<AuditTrailListDto>>.Success(paginatedTrails);
    }
}