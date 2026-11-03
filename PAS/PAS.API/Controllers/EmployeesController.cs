using Application.Features.Users.Employees.Commands;
using Application.Features.Users.Employees.Dtos;
using Application.Features.Users.Employees.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAS.API.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace PAS.API.Controllers;

[Authorize]
public class EmployeesController : BaseApiController
{
    private readonly ILogger<EmployeesController> _logger;

    public EmployeesController(ILogger<EmployeesController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all employees
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Get all employees")]
    [ProducesResponseType(typeof(ApiResponse<List<EmployeeDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<EmployeeDto>>>> GetEmployees(
        [FromQuery] string? department = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] bool? withUserAccountOnly = null)
    {
        var query = new GetEmployeesQuery
        {
            Department = department,
            SearchTerm = searchTerm,
            WithUserAccountOnly = withUserAccountOnly
        };

        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get employee by ID
    /// </summary>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get employee by ID")]
    [ProducesResponseType(typeof(ApiResponse<EmployeeDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<EmployeeDetailDto>>> GetEmployeeById(Guid id)
    {
        var query = new GetEmployeeByIdQuery(id);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get employee by user ID
    /// </summary>
    [HttpGet("by-user/{userId}")]
    [SwaggerOperation(Summary = "Get employee by user ID")]
    [ProducesResponseType(typeof(ApiResponse<EmployeeDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<EmployeeDetailDto>>> GetEmployeeByUserId(Guid userId)
    {
        var query = new GetEmployeeByUserIdQuery(userId);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new employee
    /// </summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create a new employee")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateEmployee(CreateEmployeeCommand command)
    {
        _logger.LogInformation("Creating new employee: {Code} - {Name}", command.EmployeeCode, command.FullName);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Update an existing employee
    /// </summary>
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Update an existing employee")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateEmployee(Guid id, UpdateEmployeeCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("ID mismatch", 400));
        }

        _logger.LogInformation("Updating employee: {Id}", id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete an employee
    /// </summary>
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Delete an employee")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteEmployee(Guid id)
    {
        _logger.LogInformation("Deleting employee: {Id}", id);
        var command = new DeleteEmployeeCommand(id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}