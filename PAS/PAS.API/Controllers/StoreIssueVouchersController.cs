using Application.Features.Requisition.StoreIssueVouchers.Commands;
using Application.Features.Requisition.StoreIssueVouchers.Dtos;
using Application.Features.Requisition.StoreIssueVouchers.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAS.API.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace PAS.API.Controllers;

[Authorize]
public class StoreIssueVouchersController : BaseApiController
{
    private readonly ILogger<StoreIssueVouchersController> _logger;

    public StoreIssueVouchersController(ILogger<StoreIssueVouchersController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all store issue vouchers with pagination
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Get all SIVs")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<StoreIssueVoucherDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<StoreIssueVoucherDto>>>> GetStoreIssueVouchers(
        [FromQuery] Guid? srId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetStoreIssueVouchersQuery
        {
            SRId = srId,
            FromDate = fromDate,
            ToDate = toDate,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await Mediator.Send(query);

        if (result.Succeeded && result.Data != null)
        {
            var paginatedResponse = new PaginatedResponse<StoreIssueVoucherDto>
            {
                Items = result.Data.Items,
                PageNumber = result.Data.PageNumber,
                TotalPages = result.Data.TotalPages,
                TotalCount = result.Data.TotalCount,
                HasPreviousPage = result.Data.HasPreviousPage,
                HasNextPage = result.Data.HasNextPage
            };
            return Ok(ApiResponse<PaginatedResponse<StoreIssueVoucherDto>>.SuccessResponse(paginatedResponse));
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Get store issue voucher by ID
    /// </summary>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get SIV by ID")]
    [ProducesResponseType(typeof(ApiResponse<StoreIssueVoucherDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<StoreIssueVoucherDetailDto>>> GetStoreIssueVoucherById(Guid id)
    {
        var query = new GetStoreIssueVoucherByIdQuery(id);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new store issue voucher
    /// </summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create a new SIV")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateStoreIssueVoucher(CreateStoreIssueVoucherCommand command)
    {
        _logger.LogInformation("Creating new store issue voucher for SR: {SRId}", command.SRId);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}