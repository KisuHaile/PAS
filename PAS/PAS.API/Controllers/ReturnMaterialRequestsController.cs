using Application.Features.TransferReturn.ReturnMaterialRequests.Commands;
using Application.Features.TransferReturn.ReturnMaterialRequests.Dtos;
using Application.Features.TransferReturn.ReturnMaterialRequests.Queries;
using Application.Features.TransferReturn.ReturnMaterialRequests.Queries.GetReturnRequestById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAS.API.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace PAS.API.Controllers;

[Authorize]
public class ReturnMaterialRequestsController : BaseApiController
{
    private readonly ILogger<ReturnMaterialRequestsController> _logger;

    public ReturnMaterialRequestsController(ILogger<ReturnMaterialRequestsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all return material requests with pagination
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Get all return material requests")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<ReturnListDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<ReturnListDto>>>> GetReturnRequests(
        [FromQuery] Guid? itemId = null,
        [FromQuery] string? status = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetReturnRequestsQuery
        {
            ItemId = itemId,
            Status = status,
            FromDate = fromDate,
            ToDate = toDate,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await Mediator.Send(query);

        if (result.Succeeded && result.Data != null)
        {
            var paginatedResponse = new PaginatedResponse<ReturnListDto>
            {
                Items = result.Data.Items,
                PageNumber = result.Data.PageNumber,
                TotalPages = result.Data.TotalPages,
                TotalCount = result.Data.TotalCount,
                HasPreviousPage = result.Data.HasPreviousPage,
                HasNextPage = result.Data.HasNextPage
            };
            return Ok(ApiResponse<PaginatedResponse<ReturnListDto>>.SuccessResponse(paginatedResponse));
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Get return request by ID
    /// </summary>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get return request by ID")]
    [ProducesResponseType(typeof(ApiResponse<ReturnMaterialRequestDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ReturnMaterialRequestDetailDto>>> GetReturnRequestById(Guid id)
    {
        var query = new GetReturnRequestByIdQuery(id);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new return material request
    /// </summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create a new return material request")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateReturnRequest(CreateReturnRequestCommand command)
    {
        _logger.LogInformation("Creating return request for item: {ItemId}, quantity: {Quantity}", command.ItemId, command.Quantity);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}