using Application.Features.Storage.InventoryStock.Commands;
using Application.Features.Storage.InventoryStock.Dtos;
using Application.Features.Storage.InventoryStock.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAS.API.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace PAS.API.Controllers;

[Authorize]
public class InventoryStockController : BaseApiController
{
    private readonly ILogger<InventoryStockController> _logger;

    public InventoryStockController(ILogger<InventoryStockController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all inventory stock with pagination
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Get all inventory stock")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<InventoryStockDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<InventoryStockDto>>>> GetInventoryStock(
        [FromQuery] Guid? itemId = null,
        [FromQuery] Guid? shelfId = null,
        [FromQuery] Guid? warehouseId = null,
        [FromQuery] bool? lowStockOnly = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetInventoryStockQuery
        {
            ItemId = itemId,
            ShelfId = shelfId,
            WarehouseId = warehouseId,
            LowStockOnly = lowStockOnly,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await Mediator.Send(query);

        if (result.Succeeded && result.Data != null)
        {
            var paginatedResponse = new PaginatedResponse<InventoryStockDto>
            {
                Items = result.Data.Items,
                PageNumber = result.Data.PageNumber,
                TotalPages = result.Data.TotalPages,
                TotalCount = result.Data.TotalCount,
                HasPreviousPage = result.Data.HasPreviousPage,
                HasNextPage = result.Data.HasNextPage
            };
            return Ok(ApiResponse<PaginatedResponse<InventoryStockDto>>.SuccessResponse(paginatedResponse));
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Get stock by shelf location
    /// </summary>
    [HttpGet("by-shelf/{shelfId}")]
    [SwaggerOperation(Summary = "Get stock by shelf location")]
    [ProducesResponseType(typeof(ApiResponse<List<InventoryStockDetailDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<InventoryStockDetailDto>>>> GetStockByShelf(Guid shelfId)
    {
        var query = new GetStockByShelfQuery(shelfId);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get stock by item
    /// </summary>
    [HttpGet("by-item/{itemId}")]
    [SwaggerOperation(Summary = "Get stock by item")]
    [ProducesResponseType(typeof(ApiResponse<List<InventoryStockDetailDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<InventoryStockDetailDto>>>> GetStockByItem(Guid itemId)
    {
        var query = new GetStockByItemQuery(itemId);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Reserve stock
    /// </summary>
    [HttpPost("reserve")]
    [SwaggerOperation(Summary = "Reserve stock")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ReserveStock(ReserveStockCommand command)
    {
        _logger.LogInformation("Reserving stock for item: {ItemId}, quantity: {Quantity}", command.ItemId, command.Quantity);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Release reserved stock
    /// </summary>
    [HttpPost("release")]
    [SwaggerOperation(Summary = "Release reserved stock")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ReleaseStock(ReleaseStockCommand command)
    {
        _logger.LogInformation("Releasing stock for item: {ItemId}, quantity: {Quantity}", command.ItemId, command.Quantity);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Adjust stock (add or remove)
    /// </summary>
    [HttpPost("adjust")]
    [SwaggerOperation(Summary = "Adjust stock")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AdjustStock(AdjustStockCommand command)
    {
        _logger.LogInformation("Adjusting stock for inventory: {InventoryId}, new quantity: {NewQuantity}", command.InventoryId, command.NewQuantity);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}