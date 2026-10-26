using Application.Features.Requisition.ServiceRequests.Commands;
using Application.Features.Requisition.ServiceRequests.Commands.IssueServiceRequest;
using Application.Features.Requisition.ServiceRequests.Commands.RejectServiceRequest;
using Application.Features.Requisition.ServiceRequests.Dtos;
using Application.Features.Requisition.ServiceRequests.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAS.API.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace PAS.API.Controllers;

[Authorize]
public class ServiceRequestsController : BaseApiController
{
    private readonly ILogger<ServiceRequestsController> _logger;

    public ServiceRequestsController(ILogger<ServiceRequestsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all service requests with pagination
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Get all service requests")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<ServiceRequestListDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<ServiceRequestListDto>>>> GetServiceRequests(
        [FromQuery] string? status = null,
        [FromQuery] Guid? requesterId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetServiceRequestsQuery
        {
            Status = status,
            RequesterId = requesterId,
            FromDate = fromDate,
            ToDate = toDate,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await Mediator.Send(query);

        if (result.Succeeded && result.Data != null)
        {
            var paginatedResponse = new PaginatedResponse<ServiceRequestListDto>
            {
                Items = result.Data.Items,
                PageNumber = result.Data.PageNumber,
                TotalPages = result.Data.TotalPages,
                TotalCount = result.Data.TotalCount,
                HasPreviousPage = result.Data.HasPreviousPage,
                HasNextPage = result.Data.HasNextPage
            };
            return Ok(ApiResponse<PaginatedResponse<ServiceRequestListDto>>.SuccessResponse(paginatedResponse));
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Get service request by ID
    /// </summary>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get service request by ID")]
    [ProducesResponseType(typeof(ApiResponse<ServiceRequestDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ServiceRequestDetailDto>>> GetServiceRequestById(Guid id)
    {
        var query = new GetServiceRequestByIdQuery(id);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new service request
    /// </summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create a new service request")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateServiceRequest(CreateServiceRequestCommand command)
    {
        _logger.LogInformation("Creating new service request");
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Update an existing service request
    /// </summary>
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Update an existing service request")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateServiceRequest(Guid id, UpdateServiceRequestCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("ID mismatch", 400));
        }

        _logger.LogInformation("Updating service request: {Id}", id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete a service request
    /// </summary>
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Delete a service request")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteServiceRequest(Guid id)
    {
        _logger.LogInformation("Deleting service request: {Id}", id);
        var command = new DeleteServiceRequestCommand(id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Approve a service request
    /// </summary>
    [HttpPost("{id}/approve")]
    [SwaggerOperation(Summary = "Approve a service request")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ApproveServiceRequest(Guid id, [FromBody] ApproveServiceRequestCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("ID mismatch", 400));
        }

        _logger.LogInformation("Approving service request: {Id}", id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Reject a service request
    /// </summary>
    [HttpPost("{id}/reject")]
    [SwaggerOperation(Summary = "Reject a service request")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RejectServiceRequest(Guid id, [FromBody] RejectServiceRequestCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("ID mismatch", 400));
        }

        _logger.LogInformation("Rejecting service request: {Id}", id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Issue items from service request
    /// </summary>
    [HttpPost("{id}/issue")]
    [SwaggerOperation(Summary = "Issue items from service request")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<Guid>>> IssueServiceRequest(Guid id, IssueServiceRequestCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(ApiResponse<Guid>.ErrorResponse("ID mismatch", 400));
        }

        _logger.LogInformation("Issuing service request: {Id}", id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}