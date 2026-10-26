using Application.Features.Workflow.ApprovalWorkflows.Commands.CreateWorkflow;
using Application.Features.Workflow.ApprovalWorkflows.Commands.DeleteWorkflow;
using Application.Features.Workflow.ApprovalWorkflows.Commands.UpdateWorkflow;
using Application.Features.Workflow.ApprovalWorkflows.Dtos;
using Application.Features.Workflow.ApprovalWorkflows.Queries.GetWorkflowById;
using Application.Features.Workflow.ApprovalWorkflows.Queries.GetWorkflows;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAS.API.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace PAS.API.Controllers;

[Authorize]
public class ApprovalWorkflowsController : BaseApiController
{
    private readonly ILogger<ApprovalWorkflowsController> _logger;

    public ApprovalWorkflowsController(ILogger<ApprovalWorkflowsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all approval workflows
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Get all approval workflows")]
    [ProducesResponseType(typeof(ApiResponse<List<ApprovalWorkflowDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<ApprovalWorkflowDto>>>> GetWorkflows(
        [FromQuery] string? searchTerm = null)
    {
        var query = new GetWorkflowsQuery
        {
            SearchTerm = searchTerm
        };

        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get approval workflow by ID
    /// </summary>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get approval workflow by ID")]
    [ProducesResponseType(typeof(ApiResponse<ApprovalWorkflowDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ApprovalWorkflowDetailDto>>> GetWorkflowById(Guid id)
    {
        var query = new GetWorkflowByIdQuery(id);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new approval workflow
    /// </summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create a new approval workflow")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateWorkflow(CreateWorkflowCommand command)
    {
        _logger.LogInformation("Creating new approval workflow: {Name}", command.WorkflowName);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Update an existing approval workflow
    /// </summary>
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Update an existing approval workflow")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateWorkflow(Guid id, UpdateWorkflowCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("ID mismatch", 400));
        }

        _logger.LogInformation("Updating approval workflow: {Id}", id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete an approval workflow
    /// </summary>
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Delete an approval workflow")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteWorkflow(Guid id)
    {
        _logger.LogInformation("Deleting approval workflow: {Id}", id);
        var command = new DeleteWorkflowCommand(id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}