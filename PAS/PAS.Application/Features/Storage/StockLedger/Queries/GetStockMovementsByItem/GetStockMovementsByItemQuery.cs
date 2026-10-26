using Application.Common.Security;
using Application.Features.Storage.StockLedger.Dtos;
using MediatR;

namespace Application.Features.Storage.StockLedger.Queries;

[Authorize(Permissions = Permissions.StockLedger.View)]
public record GetStockMovementsByItemQuery : IRequest<Result<StockMovementSummaryDto>>
{
    public Guid ItemId { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
}