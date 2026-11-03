using Application.Features.Requisition.StoreIssueVouchers.Dtos;
using AutoMapper;
using MediatR;

namespace Application.Features.Requisition.StoreIssueVouchers.Queries;

public class GetStoreIssueVouchersQueryHandler : IRequestHandler<GetStoreIssueVouchersQuery, Result<PaginatedList<StoreIssueVoucherDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetStoreIssueVouchersQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedList<StoreIssueVoucherDto>>> Handle(GetStoreIssueVouchersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.StoreIssueVouchers
            .Include(s => s.ServiceRequest)
            .Where(s => !s.IsDeleted)
            .AsNoTracking();

        // Apply filters
        if (request.SRId.HasValue)
        {
            query = query.Where(s => s.SRId == request.SRId);
        }

        if (request.IssuedById.HasValue)
        {
            query = query.Where(s => s.IssuedById == request.IssuedById);
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(s => s.IssueDate >= request.FromDate);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(s => s.IssueDate <= request.ToDate);
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            query = query.Where(s => s.Status == request.Status);
        }

        // Get items counts from SR_Details
        var sivList = await query
            .OrderByDescending(s => s.IssueDate)
            .ToListAsync(cancellationToken);

        var sivDtos = new List<StoreIssueVoucherDto>();

        foreach (var siv in sivList)
        {
            var dto = _mapper.Map<StoreIssueVoucherDto>(siv);

            // Get item details from service request
            var itemDetails = await _context.SR_Details
                .Where(d => d.SRId == siv.SRId && d.IssuedQty > 0)
                .ToListAsync(cancellationToken);

            dto.TotalItems = itemDetails.Count;
            dto.TotalQuantity = itemDetails.Sum(d => d.IssuedQty);

            sivDtos.Add(dto);
        }

        var paginatedList = new PaginatedList<StoreIssueVoucherDto>(
            sivDtos.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList(),
            sivDtos.Count,
            request.PageNumber,
            request.PageSize);

        return Result<PaginatedList<StoreIssueVoucherDto>>.Success(paginatedList);
    }
}