using Application.Common.Security;
using Application.Events;
using MediatR;

namespace Application.Features.Receiving.ReceivingNotes.Commands;

[Authorize(Permissions = Permissions.Receiving.Create)]
public record CreateReceivingNoteCommand : IRequest<Result<Guid>>
{
    public string GRNNumber { get; init; } = string.Empty;
    public Guid SupplierId { get; init; }
    public string PONumber { get; init; } = string.Empty;
    public string InvoiceNumber { get; init; } = string.Empty;
    public DateTime? InvoiceDate { get; init; }
    public string DeliveryNoteNumber { get; init; } = string.Empty;
    public string VehicleNumber { get; init; } = string.Empty;
    public string DriverName { get; init; } = string.Empty;
    public string Remarks { get; init; } = string.Empty;
    public List<ReceivingNoteItemDto> Items { get; init; } = new();
}

public record ReceivingNoteItemDto
{
    public Guid ItemId { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
}