using Application.Features.Reports.StockMovementReport;
using Application.Features.Reports.StockMovementReport.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAS.API.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace PAS.API.Controllers;

[Authorize]
public class StockMovementReportsController : BaseApiController
{
    private readonly ILogger<StockMovementReportsController> _logger;

    public StockMovementReportsController(ILogger<StockMovementReportsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Generate stock movement report
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Generate stock movement report")]
    [ProducesResponseType(typeof(ApiResponse<StockMovementReportDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<StockMovementReportDto>>> GetStockMovementReport(
        [FromQuery] Guid? itemId = null,
        [FromQuery] Guid? warehouseId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] string? transactionType = null)
    {
        var query = new StockMovementReportQuery
        {
            ItemId = itemId,
            WarehouseId = warehouseId,
            FromDate = fromDate ?? DateTime.UtcNow.AddMonths(-1),
            ToDate = toDate ?? DateTime.UtcNow,
            TransactionType = transactionType
        };

        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Export stock movement report to Excel
    /// </summary>
    [HttpGet("export")]
    [SwaggerOperation(Summary = "Export stock movement report to Excel")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> ExportStockMovementReport(
        [FromQuery] Guid? itemId = null,
        [FromQuery] Guid? warehouseId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] string? transactionType = null)
    {
        var query = new StockMovementReportQuery
        {
            ItemId = itemId,
            WarehouseId = warehouseId,
            FromDate = fromDate ?? DateTime.UtcNow.AddMonths(-1),
            ToDate = toDate ?? DateTime.UtcNow,
            TransactionType = transactionType
        };

        var result = await Mediator.Send(query);

        if (result.Succeeded && result.Data != null)
        {
            var excelBytes = GenerateExcelReport(result.Data);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Stock_Movement_Report_{DateTime.Now:yyyyMMdd}.xlsx");
        }

        return HandleResult(result);
    }

    private byte[] GenerateExcelReport(StockMovementReportDto data)
    {
        return Array.Empty<byte>();
    }
}