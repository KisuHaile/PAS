using Application.Features.Common.AuditTrail.Dtos;
using AutoMapper;
using MediatR;

namespace Application.Features.Common.AuditTrail.Queries.GetAuditTrailByEntity;

public class GetAuditTrailByEntityHandler : IRequestHandler<GetAuditTrailByEntityQuery, Result<List<AuditTrailDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAuditTrailByEntityHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<List<AuditTrailDto>>> Handle(GetAuditTrailByEntityQuery request, CancellationToken cancellationToken)
    {
        var trails = await _context.AuditTrails
            .Where(a => a.EntityName == request.EntityName &&
                       a.EntityId == request.EntityId &&
                       !a.IsDeleted)
            .OrderByDescending(a => a.ActionDate)
            .ProjectTo<AuditTrailDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return Result<List<AuditTrailDto>>.Success(trails);
    }
}