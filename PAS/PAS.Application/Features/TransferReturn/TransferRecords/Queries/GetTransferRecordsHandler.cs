using Application.Features.TransferReturn.TransferRecords.Dtos;
using AutoMapper;
using MediatR;

namespace Application.Features.TransferReturn.TransferRecords.Queries;

public class GetTransferRecordsQueryHandler : IRequestHandler<GetTransferRecordsQuery, Result<PaginatedList<TransferListDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetTransferRecordsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedList<TransferListDto>>> Handle(GetTransferRecordsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.TransferRecords
            .Include(t => t.Item)
            .Include(t => t.FromLocation)
            .Include(t => t.ToLocation)
            .Where(t => !t.IsDeleted)
            .AsNoTracking();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            query = query.Where(t => t.Status == request.Status);
        }

        if (request.ItemId.HasValue)
        {
            query = query.Where(t => t.ItemId == request.ItemId);
        }

        if (request.FromLocationId.HasValue)
        {
            query = query.Where(t => t.FromLocationId == request.FromLocationId);
        }

        if (request.ToLocationId.HasValue)
        {
            query = query.Where(t => t.ToLocationId == request.ToLocationId);
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(t => t.TransferDate >= request.FromDate);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(t => t.TransferDate <= request.ToDate);
        }

        if (request.InitiatedById.HasValue)
        {
            query = query.Where(t => t.InitiatedById == request.InitiatedById);
        }

        // Project to DTO
        var projectedQuery = query.Select(t => new TransferListDto
        {
            Id = t.Id,
            TransferNumber = t.TransferNumber ?? string.Empty,
            ItemName = t.Item != null ? t.Item.ItemName : string.Empty,
            Quantity = t.Quantity,
            FromLocation = t.FromLocation != null ? t.FromLocation.Name : string.Empty,
            ToLocation = t.ToLocation != null ? t.ToLocation.Name : string.Empty,
            TransferDate = t.TransferDate,
            Status = t.Status,
            InitiatedBy = string.Empty
        });

        var paginatedTransfers = await projectedQuery
            .OrderByDescending(t => t.TransferDate)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return Result<PaginatedList<TransferListDto>>.Success(paginatedTransfers);
    }
}