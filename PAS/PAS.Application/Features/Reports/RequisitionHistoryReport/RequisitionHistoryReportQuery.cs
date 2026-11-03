using Application.Common.Security;
using MediatR;
using Application.Features.Reports.RequisitionHistoryReport.Dtos;

namespace Application.Features.Reports.RequisitionHistoryReport;

[Authorize(Permissions = Permissions.Reports.View)]
public record RequisitionHistoryReportQuery : IRequest<Result<RequisitionHistoryReportDto>>
{
    public DateTime FromDate { get; init; }
    public DateTime ToDate { get; init; }
    public string? Status { get; init; }
    public string? Department { get; init; }
    public Guid? RequesterId { get; init; }
}