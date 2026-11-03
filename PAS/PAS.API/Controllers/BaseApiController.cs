using Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PAS.API.Models.Responses;

namespace PAS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
public abstract class BaseApiController : ControllerBase
{
    private ISender? _mediator;

    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    protected ActionResult HandleResult<T>(Result<T> result)
    {
        if (result == null)
        {
            return NotFound(ApiResponse<T>.ErrorResponse("Resource not found", 404));
        }

        if (result.Succeeded && result.Data != null)
        {
            return Ok(ApiResponse<T>.SuccessResponse(result.Data));
        }

        if (result.Succeeded && result.Data == null)
        {
            return NotFound(ApiResponse<T>.ErrorResponse("Resource not found", 404));
        }

        return BadRequest(ApiResponse<T>.ErrorResponse(string.Join(", ", result.Errors), 400));
    }

    protected ActionResult HandleResult(Result result)
    {
        if (result == null)
        {
            return NotFound(ApiResponse<object>.ErrorResponse("Resource not found", 404));
        }

        if (result.Succeeded)
        {
            return Ok(ApiResponse<object>.SuccessResponse(null));
        }

        return BadRequest(ApiResponse<object>.ErrorResponse(string.Join(", ", result.Errors), 400));
    }

    protected ActionResult HandlePaginatedResult<T>(PaginatedList<T> result)
    {
        if (result == null)
        {
            return NotFound(ApiResponse<PaginatedResponse<T>>.ErrorResponse("Resource not found", 404));
        }

        var response = new PaginatedResponse<T>
        {
            Items = result.Items,
            PageNumber = result.PageNumber,
            TotalPages = result.TotalPages,
            TotalCount = result.TotalCount,
            HasPreviousPage = result.HasPreviousPage,
            HasNextPage = result.HasNextPage
        };

        return Ok(ApiResponse<PaginatedResponse<T>>.SuccessResponse(response));
    }
}