using Application.Features.Storage.StockLedger.Dtos;
using Application.Features.Storage.StockLedger.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAS.API.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace PAS.API.Controllers;

[Authorize]
public class StockLedgerController : BaseApiController
{
    private readonly ILogger<StockLedgerController> _logger;

    public StockLedgerController(ILogger<StockLedgerController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get stock ledger entries with pagination
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Get stock ledger entries")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<StockLedgerDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<StockLedgerDto>>>> GetStockLedger(
        [FromQuery] Guid? itemId = null,
        [FromQuery] Guid? shelfId = null,
        [FromQuery] string? transactionType = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetStockLedgerQuery
        {
            ItemId = itemId,
            ShelfId = shelfId,
            TransactionType = transactionType,
            FromDate = fromDate,
            ToDate = toDate,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await Mediator.Send(query);

        if (result.Succeeded && result.Data != null)
        {
            var paginatedResponse = new PaginatedResponse<StockLedgerDto>
            {
                Items = result.Data.Items,
                PageNumber = result.Data.PageNumber,
                TotalPages = result.Data.TotalPages,
                TotalCount = result.Data.TotalCount,
                HasPreviousPage = result.Data.HasPreviousPage,
                HasNextPage = result.Data.HasNextPage
            };
            return Ok(ApiResponse<PaginatedResponse<StockLedgerDto>>.SuccessResponse(paginatedResponse));
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Get stock movements by item
    /// </summary>
    [HttpGet("by-item/{itemId}")]
    [SwaggerOperation(Summary = "Get stock movements by item")]
    [ProducesResponseType(typeof(ApiResponse<List<StockMovementDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<StockMovementDto>>>> GetStockMovementsByItem(Guid itemId)
    {
        var query = new GetStockMovementsByItemQuery { ItemId = itemId };
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get stock movements by date range
    /// </summary>
    [HttpGet("by-date")]
    [SwaggerOperation(Summary = "Get stock movements by date range")]
    [ProducesResponseType(typeof(ApiResponse<List<StockMovementDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<StockMovementDto>>>> GetStockMovementsByDate(
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate)
    {
        var query = new GetStockMovementsByDateQuery { FromDate = fromDate, ToDate = toDate };
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }
}