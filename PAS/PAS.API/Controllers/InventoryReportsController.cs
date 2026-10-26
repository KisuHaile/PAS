using Application.Features.Reports.InventoryValuationReport;
using Application.Features.Reports.InventoryValuationReport.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAS.API.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace PAS.API.Controllers;

[Authorize]
public class InventoryReportsController : BaseApiController
{
    private readonly ILogger<InventoryReportsController> _logger;

    public InventoryReportsController(ILogger<InventoryReportsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Generate inventory valuation report
    /// </summary>
    [HttpGet("valuation")]
    [SwaggerOperation(Summary = "Generate inventory valuation report")]
    [ProducesResponseType(typeof(ApiResponse<InventoryValuationReportDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<InventoryValuationReportDto>>> GetInventoryValuationReport(
        [FromQuery] Guid? locationId = null,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] DateTime? asOfDate = null)
    {
        var query = new InventoryValuationReportQuery
        {
            AsOfDate = asOfDate ?? DateTime.Now,
            CategoryId = categoryId
        };

        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Export inventory valuation report to Excel
    /// </summary>
    [HttpGet("valuation/export")]
    [SwaggerOperation(Summary = "Export inventory valuation report to Excel")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> ExportInventoryValuationReport(
        [FromQuery] Guid? locationId = null,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] DateTime? asOfDate = null)
    {
        var query = new InventoryValuationReportQuery
        {
            AsOfDate = asOfDate ?? DateTime.Now,
            CategoryId = categoryId
        };

        var result = await Mediator.Send(query);

        if (result.Succeeded && result.Data != null)
        {
            // Generate Excel file from result.Data
            var excelBytes = GenerateExcelReport(result.Data);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Inventory_Valuation_{DateTime.Now:yyyyMMdd}.xlsx");
        }

        return HandleResult(result);
    }

    private byte[] GenerateExcelReport(InventoryValuationReportDto data)
    {
        // Implementation for Excel generation
        // This would use a library like EPPlus or ClosedXML
        return Array.Empty<byte>();
    }
}