using Application.Features.TransferReturn.TransferRecords.Commands;
using Application.Features.TransferReturn.TransferRecords.Dtos;
using Application.Features.TransferReturn.TransferRecords.Queries;
using Application.Features.TransferReturn.TransferRecords.Queries.GetTransferRecordById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAS.API.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace PAS.API.Controllers;

[Authorize]
public class TransferRecordsController : BaseApiController
{
    private readonly ILogger<TransferRecordsController> _logger;

    public TransferRecordsController(ILogger<TransferRecordsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all transfer records with pagination
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Get all transfer records")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<TransferListDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<TransferListDto>>>> GetTransferRecords(
        [FromQuery] Guid? itemId = null,
        [FromQuery] Guid? fromLocationId = null,
        [FromQuery] Guid? toLocationId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetTransferRecordsQuery
        {
            ItemId = itemId,
            FromLocationId = fromLocationId,
            ToLocationId = toLocationId,
            FromDate = fromDate,
            ToDate = toDate,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await Mediator.Send(query);

        if (result.Succeeded && result.Data != null)
        {
            var paginatedResponse = new PaginatedResponse<TransferListDto>
            {
                Items = result.Data.Items,
                PageNumber = result.Data.PageNumber,
                TotalPages = result.Data.TotalPages,
                TotalCount = result.Data.TotalCount,
                HasPreviousPage = result.Data.HasPreviousPage,
                HasNextPage = result.Data.HasNextPage
            };
            return Ok(ApiResponse<PaginatedResponse<TransferListDto>>.SuccessResponse(paginatedResponse));
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Get transfer record by ID
    /// </summary>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get transfer record by ID")]
    [ProducesResponseType(typeof(ApiResponse<TransferRecordDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<TransferRecordDetailDto>>> GetTransferRecordById(Guid id)
    {
        var query = new GetTransferRecordByIdQuery(id);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new transfer record
    /// </summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create a new transfer record")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateTransferRecord(CreateTransferRecordCommand command)
    {
        _logger.LogInformation("Creating transfer record for item: {ItemId}", command.ItemId);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Approve a transfer
    /// </summary>
    [HttpPost("{id}/approve")]
    [SwaggerOperation(Summary = "Approve a transfer")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ApproveTransfer(Guid id, ApproveTransferCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("ID mismatch", 400));
        }

        _logger.LogInformation("Approving transfer: {Id}", id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}