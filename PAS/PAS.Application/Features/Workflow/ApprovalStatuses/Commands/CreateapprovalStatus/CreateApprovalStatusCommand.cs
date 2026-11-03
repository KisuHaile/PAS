using Application.Common.Security;
using MediatR;

namespace Application.Features.Workflow.ApprovalStatuses.Commands.CreateApprovalStatus;

[Authorize(Permissions = Permissions.Workflow.Create)]
public record CreateApprovalStatusCommand : IRequest<Result<Guid>>
{
    public string StatusName { get; init; } = string.Empty;
}