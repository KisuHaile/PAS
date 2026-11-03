using FluentValidation;

namespace Application.Features.Requisition.ServiceRequests.Commands;

public class CreateServiceRequestCommandValidator : AbstractValidator<CreateServiceRequestCommand>
{
    public CreateServiceRequestCommandValidator()
    {
        RuleFor(v => v.Items)
            .NotEmpty().WithMessage("At least one item is required.");

        RuleForEach(v => v.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ItemId)
                .NotEmpty().WithMessage("Item ID is required.");

            item.RuleFor(i => i.RequestedQty)
                .GreaterThan(0).WithMessage("Requested quantity must be greater than 0.");
        });

        RuleFor(v => v.Remarks)
            .MaximumLength(500).WithMessage("Remarks must not exceed 500 characters.");
    }
}
