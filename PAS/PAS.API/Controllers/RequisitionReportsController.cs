using Application.Features.Reports.RequisitionHistoryReport;
using Application.Features.Reports.RequisitionHistoryReport.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAS.API.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace PAS.API.Controllers;

[Authorize]
public class RequisitionReportsController : BaseApiController
{
    private readonly ILogger<RequisitionReportsController> _logger;

    public RequisitionReportsController(ILogger<RequisitionReportsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Generate requisition history report
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Generate requisition history report")]
    [ProducesResponseType(typeof(ApiResponse<RequisitionHistoryReportDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<RequisitionHistoryReportDto>>> GetRequisitionHistoryReport(
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] string? status = null,
        [FromQuery] Guid? requesterId = null)
    {
        var query = new RequisitionHistoryReportQuery
        {
            FromDate = fromDate ?? DateTime.UtcNow.AddMonths(-1),
            ToDate = toDate ?? DateTime.UtcNow,
            Status = status,
            RequesterId = requesterId
        };

        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Export requisition history report to Excel
    /// </summary>
    [HttpGet("export")]
    [SwaggerOperation(Summary = "Export requisition history report to Excel")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> ExportRequisitionHistoryReport(
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] string? status = null,
        [FromQuery] Guid? requesterId = null)
    {
        var query = new RequisitionHistoryReportQuery
        {
            FromDate = fromDate ?? DateTime.UtcNow.AddMonths(-1),
            ToDate = toDate ?? DateTime.UtcNow,
            Status = status,
            RequesterId = requesterId
        };

        var result = await Mediator.Send(query);

        if (result.Succeeded && result.Data != null)
        {
            var excelBytes = GenerateExcelReport(result.Data);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Requisition_History_{DateTime.Now:yyyyMMdd}.xlsx");
        }

        return HandleResult(result);
    }

    private byte[] GenerateExcelReport(RequisitionHistoryReportDto data)
    {
        return Array.Empty<byte>();
    }
}