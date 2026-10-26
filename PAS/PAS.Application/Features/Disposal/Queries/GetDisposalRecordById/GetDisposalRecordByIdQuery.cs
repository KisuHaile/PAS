using Application.Common.Security;
using Application.Features.Disposal.Dtos;
using MediatR;

namespace Application.Features.Disposal.Queries.GetDisposalRecordById;

[Authorize(Permissions = Permissions.Disposal.View)]
public record GetDisposalRecordByIdQuery(Guid Id) : IRequest<Result<DisposalRecordDetailDto>>;

public class GetDisposalRecordByIdQueryHandler : IRequestHandler<GetDisposalRecordByIdQuery, Result<DisposalRecordDetailDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetDisposalRecordByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<DisposalRecordDetailDto>> Handle(GetDisposalRecordByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.DisposalRecords
            .Include(d => d.Item)
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == request.Id && !d.IsDeleted, cancellationToken);

        if (entity == null)
            return Result<DisposalRecordDetailDto>.Failure("Disposal record not found.");

        return Result<DisposalRecordDetailDto>.Success(_mapper.Map<DisposalRecordDetailDto>(entity));
    }
}
