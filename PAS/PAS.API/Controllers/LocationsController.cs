using Application.Features.PropertyManagement.Locations.Commands.CreateLocation;
using Application.Features.PropertyManagement.Locations.Commands.DeleteLocation;
using Application.Features.PropertyManagement.Locations.Commands.UpdateLocation;
using Application.Features.PropertyManagement.Locations.Dtos;
using Application.Features.PropertyManagement.Locations.Queries.GetLocationById;
using Application.Features.PropertyManagement.Locations.Queries.GetLocations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAS.API.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace PAS.API.Controllers;

[Authorize]
public class LocationsController : BaseApiController
{
    private readonly ILogger<LocationsController> _logger;

    public LocationsController(ILogger<LocationsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all locations
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Get all locations")]
    [ProducesResponseType(typeof(ApiResponse<List<LocationDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<LocationDto>>>> GetLocations(
        [FromQuery] string? locationType = null,
        [FromQuery] string? searchTerm = null)
    {
        var query = new GetLocationsQuery
        {
            LocationType = locationType,
            SearchTerm = searchTerm
        };

        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get location by ID
    /// </summary>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get location by ID")]
    [ProducesResponseType(typeof(ApiResponse<LocationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<LocationDto>>> GetLocationById(Guid id)
    {
        var query = new GetLocationByIdQuery(id);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new location
    /// </summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create a new location")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateLocation(CreateLocationCommand command)
    {
        _logger.LogInformation("Creating new location: {Name}", command.Name);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Update an existing location
    /// </summary>
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Update an existing location")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateLocation(Guid id, UpdateLocationCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("ID mismatch", 400));
        }

        _logger.LogInformation("Updating location: {Id}", id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete a location
    /// </summary>
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Delete a location")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteLocation(Guid id)
    {
        _logger.LogInformation("Deleting location: {Id}", id);
        var command = new DeleteLocationCommand(id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}