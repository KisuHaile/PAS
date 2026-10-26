using Application.Common.Security;
using Application.Features.TransferReturn.ReturnMaterialRequests.Dtos;
using MediatR;

namespace Application.Features.TransferReturn.ReturnMaterialRequests.Queries;

[Authorize(Permissions = Permissions.TransferReturn.View)]
public record GetReturnRequestsQuery : IRequest<Result<PaginatedList<ReturnListDto>>>
{
    public string? Status { get; init; }
    public Guid? ItemId { get; init; }
    public string? ReturnType { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public Guid? RequestedById { get; init; }
    public Guid? SupplierId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}