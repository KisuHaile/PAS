using Application.Common.Security;
using MediatR;
using Application.Features.Reports.PropertyValuationReport.Dtos;

namespace Application.Features.Reports.PropertyValuationReport;

[Authorize(Permissions = Permissions.Reports.View)]
public record PropertyValuationReportQuery : IRequest<Result<PropertyValuationReportDto>>
{
    public DateTime? AsOfDate { get; init; }
    public Guid? LocationId { get; init; }
    public Guid? PropertyTypeId { get; init; }
    public Guid? PropertyCategoryId { get; init; }
    public decimal? MinValue { get; init; }
    public decimal? MaxValue { get; init; }
    public string? SearchTerm { get; init; }
}