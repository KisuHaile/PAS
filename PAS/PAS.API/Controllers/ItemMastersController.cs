using Application.Features.Catalog.ItemMasters.Commands.CreateItemMaster;
using Application.Features.Catalog.ItemMasters.Commands.DeleteItemMaster;
using Application.Features.Catalog.ItemMasters.Commands.UpdateItemMaster;
using Application.Features.Catalog.ItemMasters.Dtos;
using Application.Features.Catalog.ItemMasters.Queries.GetItemMasterById;
using Application.Features.Catalog.ItemMasters.Queries.GetItemMasters;
using Application.Features.Catalog.ItemMasters.Queries.GetItemsByCategory;
using Application.Features.Catalog.ItemMasters.Queries.GetLowStockItems;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAS.API.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace PAS.API.Controllers;

[Authorize]
public class ItemMastersController : BaseApiController
{
    private readonly ILogger<ItemMastersController> _logger;

    public ItemMastersController(ILogger<ItemMastersController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all items with pagination
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Get all items with pagination")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<ItemMasterListDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<ItemMasterListDto>>>> GetItems(
        [FromQuery] string? searchTerm = null,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] bool? lowStockOnly = null,
        [FromQuery] bool? requiresInspection = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetItemMastersQuery
        {
            SearchTerm = searchTerm,
            CategoryId = categoryId,
            LowStockOnly = lowStockOnly,
            RequiresInspection = requiresInspection,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await Mediator.Send(query);

        if (result.Succeeded && result.Data != null)
        {
            var paginatedResponse = new PaginatedResponse<ItemMasterListDto>
            {
                Items = result.Data.Items,
                PageNumber = result.Data.PageNumber,
                TotalPages = result.Data.TotalPages,
                TotalCount = result.Data.TotalCount,
                HasPreviousPage = result.Data.HasPreviousPage,
                HasNextPage = result.Data.HasNextPage
            };
            return Ok(ApiResponse<PaginatedResponse<ItemMasterListDto>>.SuccessResponse(paginatedResponse));
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Get item by ID
    /// </summary>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get item by ID")]
    [ProducesResponseType(typeof(ApiResponse<ItemMasterDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ItemMasterDetailDto>>> GetItemById(Guid id)
    {
        var query = new GetItemMasterByIdQuery(id);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get items by category
    /// </summary>
    [HttpGet("by-category/{categoryId}")]
    [SwaggerOperation(Summary = "Get items by category")]
    [ProducesResponseType(typeof(ApiResponse<List<ItemMasterListDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<ItemMasterListDto>>>> GetItemsByCategory(Guid categoryId)
    {
        var query = new GetItemsByCategoryQuery(categoryId);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get low stock items
    /// </summary>
    [HttpGet("low-stock")]
    [SwaggerOperation(Summary = "Get low stock items")]
    [ProducesResponseType(typeof(ApiResponse<List<LowStockItemDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<LowStockItemDto>>>> GetLowStockItems()
    {
        var query = new GetLowStockItemsQuery();
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new item
    /// </summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create a new item")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateItem(CreateItemMasterCommand command)
    {
        _logger.LogInformation("Creating new item: {Name}", command.ItemName);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Update an existing item
    /// </summary>
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Update an existing item")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateItem(Guid id, UpdateItemMasterCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("ID mismatch", 400));
        }

        _logger.LogInformation("Updating item: {Id}", id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete an item
    /// </summary>
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Delete an item")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteItem(Guid id)
    {
        _logger.LogInformation("Deleting item: {Id}", id);
        var command = new DeleteItemMasterCommand(id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}