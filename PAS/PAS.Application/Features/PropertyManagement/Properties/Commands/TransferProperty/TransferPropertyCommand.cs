using Application.Common.Security;
using MediatR;

namespace Application.Features.PropertyManagement.Properties.Commands.TransferProperty;

[Authorize(Permissions = Permissions.Properties.Transfer)]
public record TransferPropertyCommand : IRequest<Result>
{
    public Guid Id { get; init; }
    public Guid NewLocationId { get; init; }
    public Guid? NewSafetyBoxId { get; init; }
    public int? NewShelfNumber { get; init; }
    public string Remarks { get; init; } = string.Empty;
}