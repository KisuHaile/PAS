using Application.Features.Workflow.Approvers.Commands.AssignApprover;
using Application.Features.Workflow.Approvers.Commands.RemoveApprover;
using Application.Features.Workflow.Approvers.Dtos;
using Application.Features.Workflow.Approvers.Queries.GetApproversByWorkflow;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAS.API.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace PAS.API.Controllers;

[AllowAnonymous]
public class ApproversController : BaseApiController
{
    private readonly ILogger<ApproversController> _logger;

    public ApproversController(ILogger<ApproversController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get approvers by workflow
    /// </summary>
    [HttpGet("by-workflow/{workflowId}")]
    [SwaggerOperation(Summary = "Get approvers by workflow")]
    [ProducesResponseType(typeof(ApiResponse<List<ApproverDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<ApproverDto>>>> GetApproversByWorkflow(Guid workflowId)
    {
        var query = new GetApproversByWorkflowQuery(workflowId);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Assign an approver to workflow
    /// </summary>
    [HttpPost("assign")]
    [SwaggerOperation(Summary = "Assign an approver to workflow")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid>>> AssignApprover(AssignApproverCommand command)
    {
        _logger.LogInformation("Assigning approver {UserId} to workflow {WorkflowId} at level {ApprovalLevel}",
            command.UserId, command.WorkflowId, command.ApprovalLevel);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Remove an approver
    /// </summary>
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Remove an approver")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveApprover(Guid id)
    {
        _logger.LogInformation("Removing approver: {Id}", id);
        var command = new RemoveApproverCommand(id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}