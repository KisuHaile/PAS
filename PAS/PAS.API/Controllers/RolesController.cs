using Application.Features.Users.Roles.Commands.CreateRole;
using Application.Features.Users.Roles.Commands.DeleteRole;
using Application.Features.Users.Roles.Commands.UpdateRole;
using Application.Features.Users.Roles.Dtos;
using Application.Features.Users.Roles.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAS.API.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace PAS.API.Controllers;

[Authorize]
public class RolesController : BaseApiController
{
    private readonly ILogger<RolesController> _logger;

    public RolesController(ILogger<RolesController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all roles
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Get all roles")]
    [ProducesResponseType(typeof(ApiResponse<List<RoleDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<RoleDto>>>> GetRoles(
        [FromQuery] string? searchTerm = null)
    {
        var query = new GetRolesQuery
        {
            SearchTerm = searchTerm
        };

        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get user roles
    /// </summary>
    [HttpGet("user/{userId}")]
    [SwaggerOperation(Summary = "Get user roles")]
    [ProducesResponseType(typeof(ApiResponse<UserRoleDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<UserRoleDto>>> GetUserRoles(Guid userId)
    {
        var query = new GetUserRolesQuery(userId);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new role
    /// </summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create a new role")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateRole(CreateRoleCommand command)
    {
        _logger.LogInformation("Creating new role: {Name}", command.RoleName);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Update an existing role
    /// </summary>
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Update an existing role")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateRole(Guid id, UpdateRoleCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("ID mismatch", 400));
        }

        _logger.LogInformation("Updating role: {Id}", id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete a role
    /// </summary>
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Delete a role")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRole(Guid id)
    {
        _logger.LogInformation("Deleting role: {Id}", id);
        var command = new DeleteRoleCommand(id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}