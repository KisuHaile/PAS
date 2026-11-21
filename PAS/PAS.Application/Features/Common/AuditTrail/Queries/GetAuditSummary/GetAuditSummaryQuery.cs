using Application.Common.Security;
using Application.Features.Common.AuditTrail.Dtos;
using MediatR;

namespace Application.Features.Common.AuditTrail.Queries.GetAuditSummary;

[Authorize(Permissions = Permissions.AuditTrail.View)]
public record GetAuditSummaryQuery : IRequest<Result<AuditSummaryDto>>;
