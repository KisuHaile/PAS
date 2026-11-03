using Application.Common.Security;
using Application.Events;
using MediatR;

namespace Application.Features.Requisition.ServiceRequests.Commands;

[Authorize(Permissions = Permissions.Requisitions.Edit)]
public record UpdateServiceRequestCommand : IRequest<Result>
{
    public Guid Id { get; init; }
    public List<ServiceRequestItemDto> Items { get; init; } = new();
    public string? Remarks { get; init; }
}

public record ServiceRequestItemDto
{
    public Guid ItemId { get; init; }
    public Guid? SRDetailId { get; init; }
    public int RequestedQty { get; init; }
    public Guid? PreferredShelfId { get; init; }
    public string? Notes { get; init; }
}