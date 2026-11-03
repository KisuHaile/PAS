using Application.Common.Security;
using Application.Features.TransferReturn.TransferRecords.Dtos;
using MediatR;

namespace Application.Features.TransferReturn.TransferRecords.Queries;

[Authorize(Permissions = Permissions.TransferReturn.View)]
public record GetTransferRecordsQuery : IRequest<Result<PaginatedList<TransferListDto>>>
{
    public string? Status { get; init; }
    public Guid? ItemId { get; init; }
    public Guid? FromLocationId { get; init; }
    public Guid? ToLocationId { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public Guid? InitiatedById { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}