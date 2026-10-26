using Application.Features.TransferReturn.ReturnMaterialRequests.Dtos;
using AutoMapper;
using MediatR;

namespace Application.Features.TransferReturn.ReturnMaterialRequests.Queries;

public class GetReturnRequestsQueryHandler : IRequestHandler<GetReturnRequestsQuery, Result<PaginatedList<ReturnListDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetReturnRequestsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedList<ReturnListDto>>> Handle(GetReturnRequestsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.ReturnMaterialRequestNotes
            .Include(r => r.Item)
            .Where(r => !r.IsDeleted)
            .AsNoTracking();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            query = query.Where(r => r.Status == request.Status);
        }

        if (request.ItemId.HasValue)
        {
            query = query.Where(r => r.ItemId == request.ItemId);
        }

        if (!string.IsNullOrWhiteSpace(request.ReturnType))
        {
            query = query.Where(r => r.ReturnType == request.ReturnType);
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(r => r.RequestDate >= request.FromDate);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(r => r.RequestDate <= request.ToDate);
        }

        if (request.RequestedById.HasValue)
        {
            query = query.Where(r => r.RequestedById == request.RequestedById);
        }

        if (request.SupplierId.HasValue)
        {
            query = query.Where(r => r.SupplierId == request.SupplierId);
        }

        // Project to DTO
        var projectedQuery = query.Select(r => new ReturnListDto
        {
            Id = r.Id,
            ReturnNumber = r.ReturnNumber ?? string.Empty,
            ItemName = r.Item != null ? r.Item.ItemName : string.Empty,
            Quantity = r.Quantity,
            Reason = r.Reason,
            RequestDate = r.RequestDate,
            Status = r.Status,
            RequestedBy = string.Empty
        });

        var paginatedReturns = await projectedQuery
            .OrderByDescending(r => r.RequestDate)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return Result<PaginatedList<ReturnListDto>>.Success(paginatedReturns);
    }
}