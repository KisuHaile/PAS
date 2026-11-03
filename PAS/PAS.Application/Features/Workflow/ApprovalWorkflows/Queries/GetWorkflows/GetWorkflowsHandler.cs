using Application.Features.Workflow.ApprovalWorkflows.Dtos;
using AutoMapper;
using MediatR;

namespace Application.Features.Workflow.ApprovalWorkflows.Queries.GetWorkflows;

public class GetWorkflowsHandler : IRequestHandler<GetWorkflowsQuery, Result<List<ApprovalWorkflowDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetWorkflowsHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<List<ApprovalWorkflowDto>>> Handle(GetWorkflowsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.ApprovalWorkflows
            .Include(w => w.Approvers.Where(a => !a.IsDeleted))
            .Where(w => !w.IsDeleted)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(w => w.WorkflowName.Contains(request.SearchTerm) ||
                                     w.Description.Contains(request.SearchTerm));
        }

        var workflows = await query
            .OrderBy(w => w.WorkflowName)
            .ProjectTo<ApprovalWorkflowDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return Result<List<ApprovalWorkflowDto>>.Success(workflows);
    }
}