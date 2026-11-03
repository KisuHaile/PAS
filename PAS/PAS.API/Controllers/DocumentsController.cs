using Application.Features.Common.DocumentAttachments.Commands.DeleteDocument;
using Application.Features.Common.DocumentAttachments.Commands.DownloadDocument;
using Application.Features.Common.DocumentAttachments.Commands.UploadDocument;
using Application.Features.Common.DocumentAttachments.Dtos;
using Application.Features.Common.DocumentAttachments.Queries.GetDocumentById;
using Application.Features.Common.DocumentAttachments.Queries.GetDocumentsByEntity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAS.API.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace PAS.API.Controllers;

[Authorize]
public class DocumentsController : BaseApiController
{
    private readonly ILogger<DocumentsController> _logger;

    public DocumentsController(ILogger<DocumentsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get documents by entity
    /// </summary>
    [HttpGet("by-entity")]
    [SwaggerOperation(Summary = "Get documents by entity")]
    [ProducesResponseType(typeof(ApiResponse<List<DocumentAttachmentDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<DocumentAttachmentDto>>>> GetDocumentsByEntity(
        [FromQuery] string entityName,
        [FromQuery] Guid entityId)
    {
        var query = new GetDocumentsByEntityQuery
        {
            EntityName = entityName,
            EntityId = entityId
        };

        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Get document by ID
    /// </summary>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get document by ID")]
    [ProducesResponseType(typeof(ApiResponse<DocumentAttachmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<DocumentAttachmentDto>>> GetDocumentById(Guid id)
    {
        var query = new GetDocumentByIdQuery(id);
        var result = await Mediator.Send(query);
        return HandleResult(result);
    }

    /// <summary>
    /// Upload a document
    /// </summary>
    [HttpPost("upload")]
    [SwaggerOperation(Summary = "Upload a document")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid>>> UploadDocument([FromForm] UploadDocumentCommand command)
    {
        _logger.LogInformation("Uploading document: {FileName}", command.File?.FileName);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Download a document
    /// </summary>
    [HttpGet("{id}/download")]
    [SwaggerOperation(Summary = "Download a document")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadDocument(Guid id)
    {
        var command = new DownloadDocumentCommand(id);
        var result = await Mediator.Send(command);

        if (result.Succeeded && result.Data != null)
        {
            return File(result.Data.Content, result.Data.ContentType, result.Data.FileName);
        }

        return HandleResult(result);
    }

    /// <summary>
    /// Delete a document
    /// </summary>
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Delete a document")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDocument(Guid id)
    {
        _logger.LogInformation("Deleting document: {Id}", id);
        var command = new DeleteDocumentCommand(id);
        var result = await Mediator.Send(command);
        return HandleResult(result);
    }
}