using FluentValidation;

namespace Application.Features.Workflow.ApprovalWorkflows.Commands.CreateWorkflow;

public class CreateWorkflowCommandValidator : AbstractValidator<CreateWorkflowCommand>
{
    public CreateWorkflowCommandValidator()
    {
        RuleFor(v => v.WorkflowName)
            .NotEmpty().WithMessage("Workflow name is required.")
            .MaximumLength(100).WithMessage("Workflow name must not exceed 100 characters.");

        RuleFor(v => v.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
    }
}