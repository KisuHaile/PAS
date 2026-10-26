using Application.Common.Security;
using Application.Features.Storage.StockLedger.Dtos;
using MediatR;

namespace Application.Features.Storage.StockLedger.Queries;

[Authorize(Permissions = Permissions.StockLedger.View)]
public record GetStockLedgerQuery : IRequest<Result<PaginatedList<StockLedgerDto>>>
{
    public Guid? ItemId { get; init; }
    public Guid? ShelfId { get; init; }
    public Guid? WarehouseId { get; init; }
    public string? TransactionType { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public Guid? ReferenceId { get; init; }
    public string? BatchNumber { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}