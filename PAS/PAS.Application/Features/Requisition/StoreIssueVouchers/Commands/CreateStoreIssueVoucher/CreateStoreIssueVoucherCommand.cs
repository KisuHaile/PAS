using Application.Common.Security;
using Application.Events;
using Application.Features.Requisition.StoreIssueVouchers.Dtos;
using MediatR;

namespace Application.Features.Requisition.StoreIssueVouchers.Commands;

[Authorize(Permissions = Permissions.Requisitions.Issue)]
public record CreateStoreIssueVoucherCommand : IRequest<Result<Guid>>
{
    public Guid SRId { get; init; }
    public string RecipientSignature { get; init; } = string.Empty;
    public string? RecipientName { get; init; }
    public string? Remarks { get; init; }
    public List<IssueItemDto> Items { get; init; } = new();
}