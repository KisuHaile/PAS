using Application.Common.Security;
using Application.Features.PropertyManagement.SafetyBoxes.Dtos;
using MediatR;

namespace Application.Features.PropertyManagement.SafetyBoxes.Queries.GetSafetyBoxes;

[Authorize(Permissions = Permissions.SafetyBoxes.View)]
public record GetSafetyBoxesQuery : IRequest<Result<List<SafetyBoxDto>>>
{
    public Guid? LocationId { get; init; }
    public string? SearchTerm { get; init; }
}