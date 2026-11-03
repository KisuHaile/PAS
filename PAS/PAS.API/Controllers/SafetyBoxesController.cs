using Application.Features.PropertyManagement.SafetyBoxes.Commands.AddShelf;
using Application.Features.PropertyManagement.SafetyBoxes.Commands.CreateSafetyBox;
using Application.Features.PropertyManagement.SafetyBoxes.Commands.DeleteSafetyBox;
using Application.Features.PropertyManagement.SafetyBoxes.Commands.UpdateSafetyBox;
using Application.Features.PropertyManagement.SafetyBoxes.Dtos;
using Application.Features.PropertyManagement.SafetyBoxes.Queries.GetSafetyBoxById;
using Application.Features.PropertyManagement.SafetyBoxes.Queries.GetSafetyBoxes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAS.API.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace PAS.API.Controllers;

[Authorize]
public class SafetyBoxesController : BaseApiController
{
    private readonly ILogger<SafetyBoxesController> _logger;

    public SafetyBoxesController(ILogger<SafetyBoxesController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all safety boxes
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Get all safety boxes")]
    [ProducesResponseType(typeof(ApiResponse<List<SafetyBoxDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<SafetyBoxDto>>>> GetSafetyBoxes(
        [FromQuery] Guid? locationId = null,
        [FromQuery] string? searchTerm = null)
    {
        var query = new GetSafetyBoxesQuery
        {
            LocationId = locationId,
            SearchTerm = searchTerm
        };

        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get safety box by ID
    /// </summary>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get safety box by ID")]
    [ProducesResponseType(typeof(ApiResponse<SafetyBoxDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<SafetyBoxDetailDto>>> GetSafetyBoxById(Guid id)
    {
        var query = new GetSafetyBoxByIdQuery(id);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new safety box
    /// </summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create a new safety box")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateSafetyBox(CreateSafetyBoxCommand command)
    {
        _logger.LogInformation("Creating new safety box: {BoxNumber}", command.BoxNumber);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Update an existing safety box
    /// </summary>
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Update an existing safety box")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSafetyBox(Guid id, UpdateSafetyBoxCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("ID mismatch", 400));
        }

        _logger.LogInformation("Updating safety box: {Id}", id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete a safety box
    /// </summary>
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Delete a safety box")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSafetyBox(Guid id)
    {
        _logger.LogInformation("Deleting safety box: {Id}", id);
        var command = new DeleteSafetyBoxCommand(id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Add a shelf to safety box
    /// </summary>
    [HttpPost("{id}/shelves")]
    [SwaggerOperation(Summary = "Add a shelf to safety box")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<Guid>>> AddShelf(Guid id, AddShelfCommand command)
    {
        if (id != command.SafetyBoxId)
        {
            return BadRequest(ApiResponse<Guid>.ErrorResponse("ID mismatch", 400));
        }

        _logger.LogInformation("Adding shelf {ShelfNumber} to safety box: {Id}", command.ShelfNumber, id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}