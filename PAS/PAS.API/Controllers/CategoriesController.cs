using Application.Features.Catalog.Categories.Commands.CreateCategory;
using Application.Features.Catalog.Categories.Commands.DeleteCategory;
using Application.Features.Catalog.Categories.Commands.UpdateCategory;
using Application.Features.Catalog.Categories.Dtos;
using Application.Features.Catalog.Categories.Queries.GetCategories;
using Application.Features.Catalog.Categories.Queries.GetCategoryById;
using Application.Features.Catalog.Categories.Queries.GetCategoryHierarchy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAS.API.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace PAS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController : BaseApiController
{
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(ILogger<CategoriesController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all categories
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Get all categories")]
    [ProducesResponseType(typeof(ApiResponse<List<CategoryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<CategoryDto>>>> GetCategories(
        [FromQuery] bool includeInactive = false,
        [FromQuery] Guid? parentCategoryId = null,
        [FromQuery] string? searchTerm = null)
    {
        var query = new GetCategoriesQuery
        {
            IncludeInactive = includeInactive,
            ParentCategoryId = parentCategoryId,
            SearchTerm = searchTerm
        };

        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get category by ID
    /// </summary>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get category by ID")]
    [ProducesResponseType(typeof(ApiResponse<CategoryDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<CategoryDetailDto>>> GetCategoryById(Guid id)
    {
        var query = new GetCategoryByIdQuery(id);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get category hierarchy tree
    /// </summary>
    [HttpGet("hierarchy")]
    [SwaggerOperation(Summary = "Get category hierarchy tree")]
    [ProducesResponseType(typeof(ApiResponse<List<CategoryHierarchyDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<CategoryHierarchyDto>>>> GetHierarchy()
    {
        var query = new GetCategoryHierarchyQuery();
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new category
    /// </summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create a new category")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateCategory(CreateCategoryCommand command)
    {
        _logger.LogInformation("Creating new category: {Name}", command.Name);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Update an existing category
    /// </summary>
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Update an existing category")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCategory(Guid id, UpdateCategoryCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("ID mismatch", 400));
        }

        _logger.LogInformation("Updating category: {Id}", id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete a category
    /// </summary>
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Delete a category")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        _logger.LogInformation("Deleting category: {Id}", id);
        var command = new DeleteCategoryCommand(id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}