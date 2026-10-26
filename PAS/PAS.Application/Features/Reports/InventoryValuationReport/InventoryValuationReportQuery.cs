using Application.Common.Security;
using MediatR;
using Application.Features.Reports.InventoryValuationReport.Dtos;

namespace Application.Features.Reports.InventoryValuationReport;

[Authorize(Permissions = Permissions.Reports.View)]
public record InventoryValuationReportQuery : IRequest<Result<InventoryValuationReportDto>>
{
    public DateTime? AsOfDate { get; init; }
    public Guid? CategoryId { get; init; }
    public Guid? WarehouseId { get; init; }
    public bool IncludeZeroStock { get; init; }
    public string? ItemSearch { get; init; }
}