using Application.Features.Reports.PropertyValuationReport;
using Application.Features.Reports.PropertyValuationReport.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAS.API.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace PAS.API.Controllers;

[Authorize]
public class PropertyReportsController : BaseApiController
{
    private readonly ILogger<PropertyReportsController> _logger;

    public PropertyReportsController(ILogger<PropertyReportsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Generate property valuation report
    /// </summary>
    [HttpGet("valuation")]
    [SwaggerOperation(Summary = "Generate property valuation report")]
    [ProducesResponseType(typeof(ApiResponse<PropertyValuationReportDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PropertyValuationReportDto>>> GetPropertyValuationReport(
        [FromQuery] Guid? locationId = null,
        [FromQuery] Guid? propertyTypeId = null,
        [FromQuery] Guid? propertyCategoryId = null,
        [FromQuery] DateTime? asOfDate = null)
    {
        var query = new PropertyValuationReportQuery
        {
            AsOfDate = asOfDate ?? DateTime.UtcNow,
            LocationId = locationId,
            PropertyTypeId = propertyTypeId,
            PropertyCategoryId = propertyCategoryId
        };

        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Generate property issuance report
    /// </summary>
    [HttpGet("issuance")]
    [SwaggerOperation(Summary = "Generate property issuance report")]
    [ProducesResponseType(typeof(ApiResponse<PropertyIssuanceReportDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PropertyIssuanceReportDto>>> GetPropertyIssuanceReport(
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] Guid? locationId = null,
        [FromQuery] string? department = null,
        [FromQuery] Guid? employeeId = null)
    {
        var query = new PropertyIssuanceReportQuery
        {
            FromDate = fromDate ?? DateTime.UtcNow.AddMonths(-1),
            ToDate = toDate ?? DateTime.UtcNow,
            Department = department,
            EmployeeId = employeeId
        };

        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Export property valuation report to Excel
    /// </summary>
    [HttpGet("valuation/export")]
    [SwaggerOperation(Summary = "Export property valuation report to Excel")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> ExportPropertyValuationReport(
        [FromQuery] Guid? locationId = null,
        [FromQuery] Guid? propertyTypeId = null,
        [FromQuery] Guid? propertyCategoryId = null,
        [FromQuery] DateTime? asOfDate = null)
    {
        var query = new PropertyValuationReportQuery
        {
            AsOfDate = asOfDate ?? DateTime.UtcNow,
            LocationId = locationId,
            PropertyTypeId = propertyTypeId,
            PropertyCategoryId = propertyCategoryId
        };

        var result = await Mediator.Send(query);

        if (result.Succeeded && result.Data != null)
        {
            var excelBytes = GenerateExcelReport(result.Data);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Property_Valuation_{DateTime.Now:yyyyMMdd}.xlsx");
        }

        return HandleResult(result);
    }

    private byte[] GenerateExcelReport(PropertyValuationReportDto data)
    {
        return Array.Empty<byte>();
    }
}