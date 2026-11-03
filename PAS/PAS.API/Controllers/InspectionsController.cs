using Application.Features.Receiving.Inspections.Commands.CreateInspection;
using Application.Features.Receiving.Inspections.Dtos;
using Application.Features.Receiving.Inspections.Queries.GetInspectionById;
using Application.Features.Receiving.Inspections.Queries.GetInspections;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAS.API.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace PAS.API.Controllers;

[Authorize]
public class InspectionsController : BaseApiController
{
    private readonly ILogger<InspectionsController> _logger;

    public InspectionsController(ILogger<InspectionsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all inspections with pagination
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Get all inspections")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<InspectionDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<InspectionDto>>>> GetInspections(
        [FromQuery] Guid? receivingNoteId = null,
        [FromQuery] bool? isPassed = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetInspectionsQuery
        {
            ReceivingNoteId = receivingNoteId,
            IsPassed = isPassed,
            FromDate = fromDate,
            ToDate = toDate,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await Mediator.Send(query);

        if (result.Succeeded && result.Data != null)
        {
            var paginatedResponse = new PaginatedResponse<InspectionDto>
            {
                Items = result.Data.Items,
                PageNumber = result.Data.PageNumber,
                TotalPages = result.Data.TotalPages,
                TotalCount = result.Data.TotalCount,
                HasPreviousPage = result.Data.HasPreviousPage,
                HasNextPage = result.Data.HasNextPage
            };
            return Ok(ApiResponse<PaginatedResponse<InspectionDto>>.SuccessResponse(paginatedResponse));
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Get inspection by ID
    /// </summary>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get inspection by ID")]
    [ProducesResponseType(typeof(ApiResponse<InspectionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<InspectionDto>>> GetInspectionById(Guid id)
    {
        var query = new GetInspectionByIdQuery(id);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new inspection
    /// </summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create a new inspection")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateInspection(CreateInspectionCommand command)
    {
        _logger.LogInformation("Creating inspection for receiving note: {ReceivingNoteId}", command.ReceivingNoteId);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}