using Application.Common.Security;
using Application.Features.Requisition.StoreIssueVouchers.Dtos;
using MediatR;

namespace Application.Features.Requisition.StoreIssueVouchers.Queries;

[Authorize(Permissions = Permissions.Requisitions.View)]
public record GetStoreIssueVouchersQuery : IRequest<Result<PaginatedList<StoreIssueVoucherDto>>>
{
    public Guid? SRId { get; init; }
    public Guid? IssuedById { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public string? Status { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}