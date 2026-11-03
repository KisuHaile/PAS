using Application.Features.Workflow.ApprovalStatuses.Dtos;
using AutoMapper;
using MediatR;

namespace Application.Features.Workflow.ApprovalStatuses.Queries.GetApprovalStatuses;

public class GetApprovalStatusesHandler : IRequestHandler<GetApprovalStatusesQuery, Result<List<ApprovalStatusDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetApprovalStatusesHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<List<ApprovalStatusDto>>> Handle(GetApprovalStatusesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.ApprovalStatuses
            .Where(s => !s.IsDeleted)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(s => s.StatusName.Contains(request.SearchTerm));
        }

        var statuses = await query
            .OrderBy(s => s.StatusName)
            .ProjectTo<ApprovalStatusDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return Result<List<ApprovalStatusDto>>.Success(statuses);
    }
}