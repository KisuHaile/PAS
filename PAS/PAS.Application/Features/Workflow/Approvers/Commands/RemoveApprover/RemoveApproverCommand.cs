using Application.Common.Security;
using MediatR;

namespace Application.Features.Workflow.Approvers.Commands.RemoveApprover;

[Authorize(Permissions = Permissions.Workflow.Assign)]
public record RemoveApproverCommand(Guid Id) : IRequest<Result>;