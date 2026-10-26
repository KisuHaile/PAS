using Application.Common.Security;
using MediatR;

namespace Application.Features.PropertyManagement.SafetyBoxes.Commands.UpdateSafetyBox;

[Authorize(Permissions = Permissions.SafetyBoxes.Edit)]
public record UpdateSafetyBoxCommand : IRequest<Result>
{
    public Guid Id { get; init; }
    public string BoxNumber { get; init; } = string.Empty;
    public int TotalShelves { get; init; }
    public Guid LocationId { get; init; }
}