using Application.Features.PropertyManagement.SafetyBoxes.Commands.AddShelf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAS.API.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace PAS.API.Controllers;

[Authorize]
public class SafetyBoxShelvesController : BaseApiController
{
    private readonly ILogger<SafetyBoxShelvesController> _logger;

    public SafetyBoxShelvesController(ILogger<SafetyBoxShelvesController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Create a new shelf in a safety box
    /// </summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create a new shelf in a safety box")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateShelf([FromBody] AddShelfCommand command)
    {
        _logger.LogInformation("Creating shelf {ShelfNumber} in safety box: {SafetyBoxId}", command.ShelfNumber, command.SafetyBoxId);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new shelf in a specific safety box
    /// </summary>
    [HttpPost("{safetyBoxId}")]
    [SwaggerOperation(Summary = "Create a new shelf in a specific safety box")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateShelf(Guid safetyBoxId, [FromBody] AddShelfCommand command)
    {
        var request = command with { SafetyBoxId = safetyBoxId };

        _logger.LogInformation("Creating shelf {ShelfNumber} in safety box: {SafetyBoxId}", request.ShelfNumber, request.SafetyBoxId);
        var result = await Mediator.Send(request);
        return HandleResult(result);
    }
}
