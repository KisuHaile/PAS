using Application.Features.Storage.Warehouses.Commands;
using Application.Features.Storage.Warehouses.Dtos;
using Application.Features.Storage.Warehouses.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAS.API.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace PAS.API.Controllers;

[Authorize]
public class WarehousesController : BaseApiController
{
    private readonly ILogger<WarehousesController> _logger;

    public WarehousesController(ILogger<WarehousesController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all warehouses
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Get all warehouses")]
    [ProducesResponseType(typeof(ApiResponse<List<WarehouseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<WarehouseDto>>>> GetWarehouses(
        [FromQuery] string? searchTerm = null)
    {
        var query = new GetWarehousesQuery
        {
            SearchTerm = searchTerm
        };

        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get warehouse by ID
    /// </summary>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get warehouse by ID")]
    [ProducesResponseType(typeof(ApiResponse<WarehouseDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<WarehouseDetailDto>>> GetWarehouseById(Guid id)
    {
        var query = new GetWarehouseByIdQuery(id);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new warehouse
    /// </summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create a new warehouse")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateWarehouse(CreateWarehouseCommand command)
    {
        _logger.LogInformation("Creating new warehouse: {Name}", command.WarehouseName);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Update an existing warehouse
    /// </summary>
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Update an existing warehouse")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateWarehouse(Guid id, UpdateWarehouseCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("ID mismatch", 400));
        }

        _logger.LogInformation("Updating warehouse: {Id}", id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete a warehouse
    /// </summary>
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Delete a warehouse")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteWarehouse(Guid id)
    {
        _logger.LogInformation("Deleting warehouse: {Id}", id);
        var command = new DeleteWarehouseCommand(id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}