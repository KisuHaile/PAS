using Application.Common.Security;
using Application.Features.Common.AuditTrail.Dtos;
using MediatR;

namespace Application.Features.Common.AuditTrail.Queries.GetAuditTrails;

[Authorize(Permissions = Permissions.AuditTrail.View)]
public record GetAuditTrailsQuery : IRequest<Result<PaginatedList<AuditTrailListDto>>>
{
    public string? UserName { get; init; }
    public string? Action { get; init; }
    public string? EntityName { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}