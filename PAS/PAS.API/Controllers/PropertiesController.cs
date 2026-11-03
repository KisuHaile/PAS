using Application.Features.PropertyManagement.Properties.Commands.CreateProperty;
using Application.Features.PropertyManagement.Properties.Commands.DeleteProperty;
using Application.Features.PropertyManagement.Properties.Commands.TransferProperty;
using Application.Features.PropertyManagement.Properties.Commands.UpdateProperty;
using Application.Features.PropertyManagement.Properties.Dtos;
using Application.Features.PropertyManagement.Properties.Queries.GetProperties;
using Application.Features.PropertyManagement.Properties.Queries.GetPropertiesByLocation;
using Application.Features.PropertyManagement.Properties.Queries.GetPropertyById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAS.API.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace PAS.API.Controllers;

[Authorize]
public class PropertiesController : BaseApiController
{
    private readonly ILogger<PropertiesController> _logger;

    public PropertiesController(ILogger<PropertiesController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all properties with pagination
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Get all properties")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<PropertyDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<PropertyDto>>>> GetProperties(
        [FromQuery] string? searchTerm = null,
        [FromQuery] Guid? locationId = null,
        [FromQuery] Guid? propertyTypeId = null,
        [FromQuery] Guid? propertyCategoryId = null,
        [FromQuery] Guid? safetyBoxId = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] DateTime? fromPurchaseDate = null,
        [FromQuery] DateTime? toPurchaseDate = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetPropertiesQuery
        {
            SearchTerm = searchTerm,
            LocationId = locationId,
            PropertyTypeId = propertyTypeId,
            PropertyCategoryId = propertyCategoryId,
            SafetyBoxId = safetyBoxId,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            FromPurchaseDate = fromPurchaseDate,
            ToPurchaseDate = toPurchaseDate,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await Mediator.Send(query);

        if (result.Succeeded && result.Data != null)
        {
            var paginatedResponse = new PaginatedResponse<PropertyDto>
            {
                Items = result.Data.Items,
                PageNumber = result.Data.PageNumber,
                TotalPages = result.Data.TotalPages,
                TotalCount = result.Data.TotalCount,
                HasPreviousPage = result.Data.HasPreviousPage,
                HasNextPage = result.Data.HasNextPage
            };
            return Ok(ApiResponse<PaginatedResponse<PropertyDto>>.SuccessResponse(paginatedResponse));
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Get property by ID
    /// </summary>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get property by ID")]
    [ProducesResponseType(typeof(ApiResponse<PropertyDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<PropertyDetailDto>>> GetPropertyById(Guid id)
    {
        var query = new GetPropertyByIdQuery(id);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get properties by location
    /// </summary>
    [HttpGet("by-location/{locationId}")]
    [SwaggerOperation(Summary = "Get properties by location")]
    [ProducesResponseType(typeof(ApiResponse<List<PropertyDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<PropertyDto>>>> GetPropertiesByLocation(Guid locationId)
    {
        var query = new GetPropertiesByLocationQuery(locationId);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Create new property
    /// </summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create a new property")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateProperty(CreatePropertyCommand command)
    {
        _logger.LogInformation("Creating new property: {Name}", command.Name);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Update property
    /// </summary>
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Update an existing property")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProperty(Guid id, UpdatePropertyCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("ID mismatch", 400));
        }

        _logger.LogInformation("Updating property: {Id}", id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete property
    /// </summary>
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Delete a property")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProperty(Guid id)
    {
        _logger.LogInformation("Deleting property: {Id}", id);
        var command = new DeletePropertyCommand(id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Transfer property to new location
    /// </summary>
    [HttpPost("{id}/transfer")]
    [SwaggerOperation(Summary = "Transfer property to new location")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> TransferProperty(Guid id, TransferPropertyCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("ID mismatch", 400));
        }

        _logger.LogInformation("Transferring property: {Id} to new location", id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}