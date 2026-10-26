using Application.Features.Users.Authentication.Commands;
using Application.Features.Users.Authentication.Dtos;
using Application.Features.Users.Authentication.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAS.API.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace PAS.API.Controllers;

[Authorize]
public class UsersController : BaseApiController
{
    private readonly ILogger<UsersController> _logger;

    public UsersController(ILogger<UsersController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all users with pagination
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Get all users")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<UserDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<UserDto>>>> GetUsers(
        [FromQuery] string? searchTerm = null,
        [FromQuery] Guid? roleId = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetUsersQuery
        {
            SearchTerm = searchTerm,
            RoleId = roleId,
            IsActive = isActive,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await Mediator.Send(query);

        if (result.Succeeded && result.Data != null)
        {
            var paginatedResponse = new PaginatedResponse<UserDto>
            {
                Items = result.Data.Items,
                PageNumber = result.Data.PageNumber,
                TotalPages = result.Data.TotalPages,
                TotalCount = result.Data.TotalCount,
                HasPreviousPage = result.Data.HasPreviousPage,
                HasNextPage = result.Data.HasNextPage
            };
            return Ok(ApiResponse<PaginatedResponse<UserDto>>.SuccessResponse(paginatedResponse));
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get user by ID")]
    [ProducesResponseType(typeof(ApiResponse<UserDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<UserDetailDto>>> GetUserById(Guid id)
    {
        var query = new GetUserByIdQuery(id);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create a new user")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateUser(RegisterUserCommand command)
    {
        _logger.LogInformation("Creating new user: {Username}", command.Username);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Update an existing user
    /// </summary>
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Update an existing user")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUser(Guid id, UpdateUserCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("ID mismatch", 400));
        }

        _logger.LogInformation("Updating user: {Id}", id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete a user
    /// </summary>
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Delete a user")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        _logger.LogInformation("Deleting user: {Id}", id);
        var command = new DeleteUserCommand(id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Activate a user
    /// </summary>
    [HttpPost("{id}/activate")]
    [SwaggerOperation(Summary = "Activate a user")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateUser(Guid id)
    {
        _logger.LogInformation("Activating user: {Id}", id);
        var command = new ActivateUserCommand(id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Deactivate a user
    /// </summary>
    [HttpPost("{id}/deactivate")]
    [SwaggerOperation(Summary = "Deactivate a user")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateUser(Guid id)
    {
        _logger.LogInformation("Deactivating user: {Id}", id);
        var command = new DeactivateUserCommand(id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}