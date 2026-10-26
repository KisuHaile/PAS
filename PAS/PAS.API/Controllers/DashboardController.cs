using Application.Features.Dashboard.Dtos;
using Application.Features.Dashboard.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAS.API.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace PAS.API.Controllers;

[Authorize]
public class DashboardController : BaseApiController
{
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(ILogger<DashboardController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get dashboard statistics and metrics
    /// </summary>
    [HttpGet("statistics")]
    [SwaggerOperation(Summary = "Get dashboard statistics")]
    [ProducesResponseType(typeof(ApiResponse<DashboardDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<DashboardDto>>> GetStatistics()
    {
        _logger.LogInformation("Fetching dashboard statistics");
        var query = new GetDashboardStatisticsQuery();
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }
}