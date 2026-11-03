using Application.Common.Security;
using Application.Features.TransferReturn.TransferRecords.Dtos;
using MediatR;

namespace Application.Features.TransferReturn.TransferRecords.Queries.GetTransferRecordById;

[Authorize(Permissions = Permissions.TransferReturn.View)]
public record GetTransferRecordByIdQuery(Guid Id) : IRequest<Result<TransferRecordDetailDto>>;

public class GetTransferRecordByIdQueryHandler : IRequestHandler<GetTransferRecordByIdQuery, Result<TransferRecordDetailDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetTransferRecordByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<TransferRecordDetailDto>> Handle(GetTransferRecordByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.TransferRecords
            .Include(t => t.Item)
            .Include(t => t.FromLocation)
            .Include(t => t.ToLocation)
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == request.Id && !t.IsDeleted, cancellationToken);

        if (entity == null)
            return Result<TransferRecordDetailDto>.Failure("Transfer record not found.");

        return Result<TransferRecordDetailDto>.Success(_mapper.Map<TransferRecordDetailDto>(entity));
    }
}
