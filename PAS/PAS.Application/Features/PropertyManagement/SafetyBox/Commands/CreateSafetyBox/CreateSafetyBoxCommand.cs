using Application.Common.Security;
using MediatR;

namespace Application.Features.PropertyManagement.SafetyBoxes.Commands.CreateSafetyBox;

[Authorize(Permissions = Permissions.SafetyBoxes.Create)]
public record CreateSafetyBoxCommand : IRequest<Result<Guid>>
{
    public string BoxNumber { get; init; } = string.Empty;
    public int TotalShelves { get; init; }
    public Guid LocationId { get; init; }
}