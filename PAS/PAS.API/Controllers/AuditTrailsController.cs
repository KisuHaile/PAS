using Application.Common.Models;
using Application.Features.Common.AuditTrail.Dtos;
using Application.Features.Common.AuditTrail.Queries.GetAuditSummary;
using Application.Features.Common.AuditTrail.Queries.GetAuditTrailByEntity;
using Application.Features.Common.AuditTrail.Queries.GetAuditTrailFilters;
using Application.Features.Common.AuditTrail.Queries.GetAuditTrails;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAS.API.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace PAS.API.Controllers;

[Authorize]
public class AuditTrailsController : BaseApiController
{
    private readonly ILogger<AuditTrailsController> _logger;

    public AuditTrailsController(ILogger<AuditTrailsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get audit trails (paginated)
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Get audit trails (paginated)")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<AuditTrailListDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<AuditTrailListDto>>>> GetAuditTrails(
        [FromQuery] string? userName = null,
        [FromQuery] string? action = null,
        [FromQuery] string? entityName = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetAuditTrailsQuery
        {
            UserName = userName,
            Action = action,
            EntityName = entityName,
            FromDate = fromDate,
            ToDate = toDate,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await Mediator.Send(query);

        if (result.Succeeded && result.Data != null)
        {
            var paginatedResponse = new PaginatedResponse<AuditTrailListDto>
            {
                Items = result.Data.Items,
                PageNumber = result.Data.PageNumber,
                TotalPages = result.Data.TotalPages,
                TotalCount = result.Data.TotalCount,
                HasPreviousPage = result.Data.HasPreviousPage,
                HasNextPage = result.Data.HasNextPage
            };
            return Ok(ApiResponse<PaginatedResponse<AuditTrailListDto>>.SuccessResponse(paginatedResponse));
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Get audit trails by entity
    /// </summary>
    [HttpGet("by-entity")]
    [SwaggerOperation(Summary = "Get audit trails by entity")]
    [ProducesResponseType(typeof(ApiResponse<List<AuditTrailDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<List<AuditTrailDto>>>> GetAuditTrailByEntity(
        [FromQuery] string entityName,
        [FromQuery] Guid entityId)
    {
        var query = new GetAuditTrailByEntityQuery
        {
            EntityName = entityName,
            EntityId = entityId
        };

        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get audit trail summary metrics
    /// </summary>
    [HttpGet("summary")]
    [SwaggerOperation(Summary = "Get audit trail summary metrics")]
    [ProducesResponseType(typeof(ApiResponse<AuditSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<AuditSummaryDto>>> GetAuditSummary()
    {
        var query = new GetAuditSummaryQuery();
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get distinct action and entity filter values
    /// </summary>
    [HttpGet("filters")]
    [SwaggerOperation(Summary = "Get distinct action and entity filter values")]
    [ProducesResponseType(typeof(ApiResponse<AuditTrailFiltersDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<AuditTrailFiltersDto>>> GetAuditFilters()
    {
        var query = new GetAuditTrailFiltersQuery();
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }
}
