using Application.Common.Security;
using Application.Features.Common.AuditTrail.Dtos;
using MediatR;

namespace Application.Features.Common.AuditTrail.Queries.GetAuditTrailFilters;

[Authorize(Permissions = Permissions.AuditTrail.View)]
public record GetAuditTrailFiltersQuery : IRequest<Result<AuditTrailFiltersDto>>;
