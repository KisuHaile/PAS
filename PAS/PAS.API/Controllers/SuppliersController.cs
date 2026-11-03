using Application.Features.Receiving.Suppliers.Commands.CreateSupplier;
using Application.Features.Receiving.Suppliers.Commands.DeleteSupplier;
using Application.Features.Receiving.Suppliers.Commands.UpdateSupplier;
using Application.Features.Receiving.Suppliers.Dtos;
using Application.Features.Receiving.Suppliers.Queries.GetSupplierById;
using Application.Features.Receiving.Suppliers.Queries.GetSuppliers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAS.API.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace PAS.API.Controllers;

[Authorize]
public class SuppliersController : BaseApiController
{
    private readonly ILogger<SuppliersController> _logger;

    public SuppliersController(ILogger<SuppliersController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all suppliers
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Get all suppliers")]
    [ProducesResponseType(typeof(ApiResponse<List<SupplierDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<SupplierDto>>>> GetSuppliers(
        [FromQuery] string? searchTerm = null)
    {
        var query = new GetSuppliersQuery
        {
            SearchTerm = searchTerm
        };

        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get supplier by ID
    /// </summary>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get supplier by ID")]
    [ProducesResponseType(typeof(ApiResponse<SupplierDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<SupplierDetailDto>>> GetSupplierById(Guid id)
    {
        var query = new GetSupplierByIdQuery(id);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new supplier
    /// </summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create a new supplier")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateSupplier(CreateSupplierCommand command)
    {
        _logger.LogInformation("Creating new supplier: {Name}", command.SupplierName);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Update an existing supplier
    /// </summary>
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Update an existing supplier")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSupplier(Guid id, UpdateSupplierCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("ID mismatch", 400));
        }

        _logger.LogInformation("Updating supplier: {Id}", id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete a supplier
    /// </summary>
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Delete a supplier")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSupplier(Guid id)
    {
        _logger.LogInformation("Deleting supplier: {Id}", id);
        var command = new DeleteSupplierCommand(id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}