using Application.Features.Workflow.ApprovalStatuses.Dtos;
using AutoMapper;
using MediatR;

namespace Application.Features.Workflow.ApprovalStatuses.Queries.GetApprovalStatusById;

public class GetApprovalStatusByIdHandler : IRequestHandler<GetApprovalStatusByIdQuery, Result<ApprovalStatusDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetApprovalStatusByIdHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<ApprovalStatusDto>> Handle(GetApprovalStatusByIdQuery request, CancellationToken cancellationToken)
    {
        var status = await _context.ApprovalStatuses
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.Id && !s.IsDeleted, cancellationToken);

        if (status == null)
        {
            throw new NotFoundException(nameof(ApprovalStatus), request.Id);
        }

        var statusDto = _mapper.Map<ApprovalStatusDto>(status);

        return Result<ApprovalStatusDto>.Success(statusDto);
    }
}