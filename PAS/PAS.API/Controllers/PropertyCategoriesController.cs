using Application.Features.PropertyManagement.PropertyCategories.Commands.CreatePropertyCategory;
using Application.Features.PropertyManagement.PropertyCategories.Commands.DeletePropertyCategory;
using Application.Features.PropertyManagement.PropertyCategories.Commands.UpdatePropertyCategory;
using Application.Features.PropertyManagement.PropertyCategories.Dtos;
using Application.Features.PropertyManagement.PropertyCategories.Queries.GetPropertyCategories;
using Application.Features.PropertyManagement.PropertyCategories.Queries.GetPropertyCategoryById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAS.API.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace PAS.API.Controllers;

[Authorize]
public class PropertyCategoriesController : BaseApiController
{
    private readonly ILogger<PropertyCategoriesController> _logger;

    public PropertyCategoriesController(ILogger<PropertyCategoriesController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all property categories
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Get all property categories")]
    [ProducesResponseType(typeof(ApiResponse<List<PropertyCategoryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<PropertyCategoryDto>>>> GetPropertyCategories(
        [FromQuery] string? searchTerm = null)
    {
        var query = new GetPropertyCategoriesQuery
        {
            SearchTerm = searchTerm
        };

        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get property category by ID
    /// </summary>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get property category by ID")]
    [ProducesResponseType(typeof(ApiResponse<PropertyCategoryDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<PropertyCategoryDetailDto>>> GetPropertyCategoryById(Guid id)
    {
        var query = new GetPropertyCategoryByIdQuery(id);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new property category
    /// </summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create a new property category")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid>>> CreatePropertyCategory(CreatePropertyCategoryCommand command)
    {
        _logger.LogInformation("Creating new property category: {Name}", command.Name);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Update an existing property category
    /// </summary>
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Update an existing property category")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePropertyCategory(Guid id, UpdatePropertyCategoryCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("ID mismatch", 400));
        }

        _logger.LogInformation("Updating property category: {Id}", id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete a property category
    /// </summary>
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Delete a property category")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePropertyCategory(Guid id)
    {
        _logger.LogInformation("Deleting property category: {Id}", id);
        var command = new DeletePropertyCategoryCommand(id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}