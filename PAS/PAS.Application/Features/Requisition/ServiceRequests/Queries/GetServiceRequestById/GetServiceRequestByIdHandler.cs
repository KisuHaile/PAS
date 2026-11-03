using Application.Features.Requisition.ServiceRequests.Dtos;
using MediatR;

namespace Application.Features.Requisition.ServiceRequests.Queries;

public class GetServiceRequestByIdQueryHandler : IRequestHandler<GetServiceRequestByIdQuery, Result<ServiceRequestDetailDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetServiceRequestByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<ServiceRequestDetailDto>> Handle(GetServiceRequestByIdQuery request, CancellationToken cancellationToken)
    {
        var serviceRequest = await _context.ServiceRequests
            .Include(s => s.Details)
                .ThenInclude(d => d.Item)
            .Include(s => s.Details)
                .ThenInclude(d => d.ShelfLocation)
            .Include(s => s.StoreIssueVoucher)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.Id && !s.IsDeleted, cancellationToken);

        if (serviceRequest == null)
        {
            throw new NotFoundException(nameof(Domain.Requisition.ServiceRequest), request.Id);
        }

        var requestDto = _mapper.Map<ServiceRequestDetailDto>(serviceRequest);

        // Get available stock for each item
        foreach (var item in requestDto.Items)
        {
            var availableStock = await _context.InventoryStocks
                .Where(i => i.ItemId == item.ItemId && !i.IsDeleted)
                .SumAsync(i => i.CurrentQuantity - i.ReservedQuantity, cancellationToken);

            item.AvailableStock = availableStock;
        }

        return Result<ServiceRequestDetailDto>.Success(requestDto);
    }
}