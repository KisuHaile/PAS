using Application.Features.PropertyManagement.PropertyTypes.Commands.CreatePropertyType;
using Application.Features.PropertyManagement.PropertyTypes.Commands.DeletePropertyType;
using Application.Features.PropertyManagement.PropertyTypes.Commands.UpdatePropertyType;
using Application.Features.PropertyManagement.PropertyTypes.Dtos;
using Application.Features.PropertyManagement.PropertyTypes.Queries.GetPropertyTypeById;
using Application.Features.PropertyManagement.PropertyTypes.Queries.GetPropertyTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAS.API.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace PAS.API.Controllers;

[Authorize]
public class PropertyTypesController : BaseApiController
{
    private readonly ILogger<PropertyTypesController> _logger;

    public PropertyTypesController(ILogger<PropertyTypesController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all property types
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Get all property types")]
    [ProducesResponseType(typeof(ApiResponse<List<PropertyTypeDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<PropertyTypeDto>>>> GetPropertyTypes(
        [FromQuery] string? searchTerm = null)
    {
        var query = new GetPropertyTypesQuery
        {
            SearchTerm = searchTerm
        };

        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get property type by ID
    /// </summary>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get property type by ID")]
    [ProducesResponseType(typeof(ApiResponse<PropertyTypeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<PropertyTypeDto>>> GetPropertyTypeById(Guid id)
    {
        var query = new GetPropertyTypeByIdQuery(id);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new property type
    /// </summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create a new property type")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid>>> CreatePropertyType(CreatePropertyTypeCommand command)
    {
        _logger.LogInformation("Creating new property type: {Name}", command.Name);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Update an existing property type
    /// </summary>
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Update an existing property type")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePropertyType(Guid id, UpdatePropertyTypeCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("ID mismatch", 400));
        }

        _logger.LogInformation("Updating property type: {Id}", id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete a property type
    /// </summary>
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Delete a property type")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePropertyType(Guid id)
    {
        _logger.LogInformation("Deleting property type: {Id}", id);
        var command = new DeletePropertyTypeCommand(id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}