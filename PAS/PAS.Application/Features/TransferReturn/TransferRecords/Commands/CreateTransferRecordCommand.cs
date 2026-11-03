using Application.Common.Security;
using Application.Events;
using Application.Features.TransferReturn.TransferRecords.Dtos;
using MediatR;

namespace Application.Features.TransferReturn.TransferRecords.Commands;

[Authorize(Permissions = Permissions.TransferReturn.Create)]
public record CreateTransferRecordCommand : IRequest<Result<Guid>>
{
    public Guid ItemId { get; init; }
    public int Quantity { get; init; }
    public Guid ToLocationId { get; init; }
    public Guid? ToShelfId { get; init; }
    public string? BatchNumber { get; init; }
    public DateTime? ExpiryDate { get; init; }
    public string Reason { get; init; } = string.Empty;
    public string? Remarks { get; init; }
    public string? Reference { get; init; }
}