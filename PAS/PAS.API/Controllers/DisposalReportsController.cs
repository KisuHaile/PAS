using Application.Features.Reports.DisposalReport;
using Application.Features.Reports.DisposalReport.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAS.API.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace PAS.API.Controllers;

[Authorize]
public class DisposalReportsController : BaseApiController
{
    private readonly ILogger<DisposalReportsController> _logger;

    public DisposalReportsController(ILogger<DisposalReportsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Generate disposal report
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Generate disposal report")]
    [ProducesResponseType(typeof(ApiResponse<DisposalReportDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<DisposalReportDto>>> GetDisposalReport(
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] string? reason = null,
        [FromQuery] Guid? itemId = null)
    {
        var query = new DisposalReportQuery
        {
            FromDate = fromDate ?? DateTime.UtcNow.AddMonths(-1),
            ToDate = toDate ?? DateTime.UtcNow,
            Reason = reason
        };

        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Export disposal report to Excel
    /// </summary>
    [HttpGet("export")]
    [SwaggerOperation(Summary = "Export disposal report to Excel")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> ExportDisposalReport(
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] string? reason = null,
        [FromQuery] Guid? itemId = null)
    {
        var query = new DisposalReportQuery
        {
            FromDate = fromDate ?? DateTime.UtcNow.AddMonths(-1),
            ToDate = toDate ?? DateTime.UtcNow,
            Reason = reason
        };

        var result = await Mediator.Send(query);

        if (result.Succeeded && result.Data != null)
        {
            var excelBytes = GenerateExcelReport(result.Data);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Disposal_Report_{DateTime.Now:yyyyMMdd}.xlsx");
        }

        return HandleResult(result);
    }

    private byte[] GenerateExcelReport(DisposalReportDto data)
    {
        return Array.Empty<byte>();
    }
}