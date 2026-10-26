using Application.Features.Receiving.ReceivingNotes.Commands;
using Application.Features.Receiving.ReceivingNotes.Dtos;
using Application.Features.Receiving.ReceivingNotes.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAS.API.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace PAS.API.Controllers;

[Authorize]
public class ReceivingNotesController : BaseApiController
{
    private readonly ILogger<ReceivingNotesController> _logger;

    public ReceivingNotesController(ILogger<ReceivingNotesController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all receiving notes with pagination
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Get all receiving notes")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<ReceivingNoteListDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<ReceivingNoteListDto>>>> GetReceivingNotes(
        [FromQuery] string? status = null,
        [FromQuery] Guid? supplierId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetReceivingNotesQuery
        {
            Status = status,
            SupplierId = supplierId,
            FromDate = fromDate,
            ToDate = toDate,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await Mediator.Send(query);

        if (result.Succeeded && result.Data != null)
        {
            var paginatedResponse = new PaginatedResponse<ReceivingNoteListDto>
            {
                Items = result.Data.Items,
                PageNumber = result.Data.PageNumber,
                TotalPages = result.Data.TotalPages,
                TotalCount = result.Data.TotalCount,
                HasPreviousPage = result.Data.HasPreviousPage,
                HasNextPage = result.Data.HasNextPage
            };
            return Ok(ApiResponse<PaginatedResponse<ReceivingNoteListDto>>.SuccessResponse(paginatedResponse));
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Get receiving note by ID
    /// </summary>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get receiving note by ID")]
    [ProducesResponseType(typeof(ApiResponse<ReceivingNoteDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ReceivingNoteDetailDto>>> GetReceivingNoteById(Guid id)
    {
        var query = new GetReceivingNoteByIdQuery(id);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new receiving note
    /// </summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create a new receiving note")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateReceivingNote(CreateReceivingNoteCommand command)
    {
        _logger.LogInformation("Creating new receiving note: {GRNNumber}", command.GRNNumber);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Approve receiving note
    /// </summary>
    [HttpPost("{id}/approve")]
    [SwaggerOperation(Summary = "Approve receiving note")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ApproveReceivingNote(Guid id, ApproveReceivingNoteCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("ID mismatch", 400));
        }

        _logger.LogInformation("Approving receiving note: {Id}", id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}