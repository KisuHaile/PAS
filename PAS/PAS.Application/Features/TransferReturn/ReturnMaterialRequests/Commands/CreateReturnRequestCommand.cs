using Application.Common.Security;
using Application.Events;
using Application.Features.TransferReturn.ReturnMaterialRequests.Dtos;
using MediatR;

namespace Application.Features.TransferReturn.ReturnMaterialRequests.Commands;

[Authorize(Permissions = Permissions.TransferReturn.Create)]
public record CreateReturnRequestCommand : IRequest<Result<Guid>>
{
    public Guid ItemId { get; init; }
    public int Quantity { get; init; }
    public string Reason { get; init; } = string.Empty;
    public string ReturnType { get; init; } = string.Empty;
    public Guid? SourceShelfId { get; init; }
    public string? BatchNumber { get; init; }
    public DateTime? ExpiryDate { get; init; }
    public string? Reference { get; init; }
    public string? Remarks { get; init; }
}