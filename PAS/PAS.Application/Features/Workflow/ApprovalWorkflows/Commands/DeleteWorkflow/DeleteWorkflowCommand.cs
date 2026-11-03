using Application.Common.Security;
using MediatR;

namespace Application.Features.Workflow.ApprovalWorkflows.Commands.DeleteWorkflow;

[Authorize(Permissions = Permissions.Workflow.Delete)]
public record DeleteWorkflowCommand(Guid Id) : IRequest<Result>;