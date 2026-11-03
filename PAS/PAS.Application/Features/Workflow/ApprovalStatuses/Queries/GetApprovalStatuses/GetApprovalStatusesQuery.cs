using Application.Common.Security;
using Application.Features.Workflow.ApprovalStatuses.Dtos;
using MediatR;

namespace Application.Features.Workflow.ApprovalStatuses.Queries.GetApprovalStatuses;

[Authorize(Permissions = Permissions.Workflow.View)]
public record GetApprovalStatusesQuery : IRequest<Result<List<ApprovalStatusDto>>>
{
    public string? SearchTerm { get; init; }
}