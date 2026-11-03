using Application.Common.Security;
using Application.Features.Reports.StockMovementReport.Dtos;
using MediatR;

namespace Application.Features.Reports.StockMovementReport;

[Authorize(Permissions = Permissions.Reports.View)]
public record StockMovementReportQuery : IRequest<Result<StockMovementReportDto>>
{
    public DateTime FromDate { get; init; }
    public DateTime ToDate { get; init; }
    public Guid? ItemId { get; init; }
    public Guid? WarehouseId { get; init; }
    public string? TransactionType { get; init; }
}
