using Application.Common.Security;
using Application.Features.Common.AuditTrail.Dtos;
using MediatR;

namespace Application.Features.Common.AuditTrail.Queries.GetAuditTrailByEntity;

[Authorize(Permissions = Permissions.AuditTrail.View)]
public record GetAuditTrailByEntityQuery : IRequest<Result<List<AuditTrailDto>>>
{
    public string EntityName { get; init; } = string.Empty;
    public Guid EntityId { get; init; }
}