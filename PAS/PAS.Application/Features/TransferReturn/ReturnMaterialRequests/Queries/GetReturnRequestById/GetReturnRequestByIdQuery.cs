using Application.Common.Security;
using Application.Features.TransferReturn.ReturnMaterialRequests.Dtos;
using MediatR;

namespace Application.Features.TransferReturn.ReturnMaterialRequests.Queries.GetReturnRequestById;

[Authorize(Permissions = Permissions.TransferReturn.View)]
public record GetReturnRequestByIdQuery(Guid Id) : IRequest<Result<ReturnMaterialRequestDetailDto>>;

public class GetReturnRequestByIdQueryHandler : IRequestHandler<GetReturnRequestByIdQuery, Result<ReturnMaterialRequestDetailDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetReturnRequestByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<ReturnMaterialRequestDetailDto>> Handle(GetReturnRequestByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.ReturnMaterialRequestNotes
            .Include(r => r.Item)
            .Include(r => r.SourceLocation)
            .Include(r => r.SourceShelf)
            .Include(r => r.Supplier)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == request.Id && !r.IsDeleted, cancellationToken);

        if (entity == null)
            return Result<ReturnMaterialRequestDetailDto>.Failure("Return request not found.");

        return Result<ReturnMaterialRequestDetailDto>.Success(_mapper.Map<ReturnMaterialRequestDetailDto>(entity));
    }
}
