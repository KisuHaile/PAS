using Application.Common.Security;
using Application.Features.Reports.PropertyValuationReport.Dtos;
using MediatR;

namespace Application.Features.Reports.PropertyValuationReport;

[Authorize(Permissions = Permissions.Reports.View)]
public record PropertyIssuanceReportQuery : IRequest<Result<PropertyIssuanceReportDto>>
{
    public DateTime FromDate { get; init; }
    public DateTime ToDate { get; init; }
    public string? Department { get; init; }
    public Guid? EmployeeId { get; init; }
}
