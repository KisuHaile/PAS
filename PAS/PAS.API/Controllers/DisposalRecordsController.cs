using Application.Features.Disposal.Commands.ApproveDisposal;
using Application.Features.Disposal.Commands.CreateDisposalRecord;
using Application.Features.Disposal.Dtos;
using Application.Features.Disposal.Queries.GetDisposalRecordById;
using Application.Features.Disposal.Queries.GetDisposalRecords;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAS.API.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace PAS.API.Controllers;

[Authorize]
public class DisposalRecordsController : BaseApiController
{
    private readonly ILogger<DisposalRecordsController> _logger;

    public DisposalRecordsController(ILogger<DisposalRecordsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all disposal records with pagination
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Get all disposal records")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<DisposalRecordDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<DisposalRecordDto>>>> GetDisposalRecords(
        [FromQuery] Guid? itemId = null,
        [FromQuery] string? status = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetDisposalRecordsQuery
        {
            ItemId = itemId,
            Status = status,
            FromDate = fromDate,
            ToDate = toDate,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await Mediator.Send(query);

        if (result.Succeeded && result.Data != null)
        {
            var paginatedResponse = new PaginatedResponse<DisposalRecordDto>
            {
                Items = result.Data.Items,
                PageNumber = result.Data.PageNumber,
                TotalPages = result.Data.TotalPages,
                TotalCount = result.Data.TotalCount,
                HasPreviousPage = result.Data.HasPreviousPage,
                HasNextPage = result.Data.HasNextPage
            };
            return Ok(ApiResponse<PaginatedResponse<DisposalRecordDto>>.SuccessResponse(paginatedResponse));
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Get disposal record by ID
    /// </summary>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get disposal record by ID")]
    [ProducesResponseType(typeof(ApiResponse<DisposalRecordDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<DisposalRecordDetailDto>>> GetDisposalRecordById(Guid id)
    {
        var query = new GetDisposalRecordByIdQuery(id);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new disposal record
    /// </summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create a new disposal record")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateDisposalRecord(CreateDisposalRecordCommand command)
    {
        _logger.LogInformation("Creating disposal record");
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Approve a disposal record
    /// </summary>
    [HttpPost("{id}/approve")]
    [SwaggerOperation(Summary = "Approve a disposal record")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ApproveDisposal(Guid id, ApproveDisposalCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("ID mismatch", 400));
        }

        _logger.LogInformation("Approving disposal record: {Id}", id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}