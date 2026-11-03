using Application.Features.Storage.ShelfLocations.Commands;
using Application.Features.Storage.ShelfLocations.Dtos;
using Application.Features.Storage.ShelfLocations.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAS.API.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace PAS.API.Controllers;

[Authorize]
public class ShelfLocationsController : BaseApiController
{
    private readonly ILogger<ShelfLocationsController> _logger;

    public ShelfLocationsController(ILogger<ShelfLocationsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all shelf locations
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Get all shelf locations")]
    [ProducesResponseType(typeof(ApiResponse<List<ShelfLocationDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<ShelfLocationDto>>>> GetShelfLocations(
        [FromQuery] Guid? warehouseId = null,
        [FromQuery] string? searchTerm = null)
    {
        var query = new GetShelfLocationsQuery
        {
            WarehouseId = warehouseId,
            SearchTerm = searchTerm
        };

        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get shelf location by ID
    /// </summary>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get shelf location by ID")]
    [ProducesResponseType(typeof(ApiResponse<ShelfLocationDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ShelfLocationDetailDto>>> GetShelfLocationById(Guid id)
    {
        var query = new GetShelfLocationByIdQuery(id);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new shelf location
    /// </summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create a new shelf location")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateShelfLocation(CreateShelfLocationCommand command)
    {
        _logger.LogInformation("Creating new shelf location at warehouse: {WarehouseId}", command.WarehouseId);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Update an existing shelf location
    /// </summary>
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Update an existing shelf location")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateShelfLocation(Guid id, UpdateShelfLocationCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("ID mismatch", 400));
        }

        _logger.LogInformation("Updating shelf location: {Id}", id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete a shelf location
    /// </summary>
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Delete a shelf location")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteShelfLocation(Guid id)
    {
        _logger.LogInformation("Deleting shelf location: {Id}", id);
        var command = new DeleteShelfLocationCommand(id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}