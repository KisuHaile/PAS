using Application.Features.Common.AuditTrail.Dtos;
using MediatR;

namespace Application.Features.Common.AuditTrail.Queries.GetAuditTrailFilters;

public class GetAuditTrailFiltersHandler : IRequestHandler<GetAuditTrailFiltersQuery, Result<AuditTrailFiltersDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAuditTrailFiltersHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<AuditTrailFiltersDto>> Handle(GetAuditTrailFiltersQuery request, CancellationToken cancellationToken)
    {
        var actions = await _context.AuditTrails
            .Where(a => !a.IsDeleted && a.Action != null && a.Action != "")
            .Select(a => a.Action)
            .Distinct()
            .OrderBy(a => a)
            .ToListAsync(cancellationToken);

        var entityNames = await _context.AuditTrails
            .Where(a => !a.IsDeleted && a.EntityName != null && a.EntityName != "")
            .Select(a => a.EntityName)
            .Distinct()
            .OrderBy(e => e)
            .ToListAsync(cancellationToken);

        var result = new AuditTrailFiltersDto
        {
            Actions = actions,
            EntityNames = entityNames
        };

        return Result<AuditTrailFiltersDto>.Success(result);
    }
}
