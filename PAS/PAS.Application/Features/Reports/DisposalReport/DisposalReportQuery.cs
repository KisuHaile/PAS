using Application.Common.Security;
using MediatR;
using Application.Features.Reports.DisposalReport.Dtos;

namespace Application.Features.Reports.DisposalReport;

[Authorize(Permissions = Permissions.Reports.View)]
public record DisposalReportQuery : IRequest<Result<DisposalReportDto>>
{
    public DateTime FromDate { get; init; }
    public DateTime ToDate { get; init; }
    public string? Status { get; init; }
    public string? Reason { get; init; }
    public string? Method { get; init; }
    public Guid? DisposedBy { get; init; }
}