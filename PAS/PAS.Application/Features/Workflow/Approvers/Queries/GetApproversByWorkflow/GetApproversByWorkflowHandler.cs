using Application.Features.Workflow.Approvers.Dtos;
using AutoMapper;
using MediatR;

namespace Application.Features.Workflow.Approvers.Queries.GetApproversByWorkflow;

public class GetApproversByWorkflowHandler : IRequestHandler<GetApproversByWorkflowQuery, Result<List<ApproverDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetApproversByWorkflowHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<List<ApproverDto>>> Handle(GetApproversByWorkflowQuery request, CancellationToken cancellationToken)
    {
        var approvers = await _context.Approvers
            .Include(a => a.Workflow)
            .Where(a => a.WorkflowId == request.WorkflowId && !a.IsDeleted)
            .OrderBy(a => a.ApprovalLevel)
            .ProjectTo<ApproverDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return Result<List<ApproverDto>>.Success(approvers);
    }
}