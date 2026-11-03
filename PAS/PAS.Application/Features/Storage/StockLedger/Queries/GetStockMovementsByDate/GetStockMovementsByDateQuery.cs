using Application.Common.Security;
using Application.Features.Storage.StockLedger.Dtos;
using MediatR;

namespace Application.Features.Storage.StockLedger.Queries;

[Authorize(Permissions = Permissions.StockLedger.View)]
public record GetStockMovementsByDateQuery : IRequest<Result<List<StockMovementByDateDto>>>
{
    public DateTime FromDate { get; init; }
    public DateTime ToDate { get; init; }
    public Guid? ItemId { get; init; }
    public Guid? WarehouseId { get; init; }
    public string? GroupBy { get; init; } = "day"; // day, week, month
}