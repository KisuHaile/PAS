using FluentValidation;

namespace Application.Features.Requisition.StoreIssueVouchers.Commands;

public class CreateStoreIssueVoucherCommandValidator : AbstractValidator<CreateStoreIssueVoucherCommand>
{
    public CreateStoreIssueVoucherCommandValidator()
    {
        RuleFor(v => v.SRId)
            .NotEmpty().WithMessage("Service request ID is required.");

        RuleFor(v => v.RecipientSignature)
            .NotEmpty().WithMessage("Recipient signature is required.")
            .MaximumLength(500).WithMessage("Recipient signature must not exceed 500 characters.");

        RuleFor(v => v.RecipientName)
            .MaximumLength(100).WithMessage("Recipient name must not exceed 100 characters.");

        RuleFor(v => v.Remarks)
            .MaximumLength(500).WithMessage("Remarks must not exceed 500 characters.");

        RuleFor(v => v.Items)
            .NotEmpty().WithMessage("At least one item is required.");

        RuleForEach(v => v.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.SRDetailId)
                .NotEmpty().WithMessage("Service request detail ID is required.");

            item.RuleFor(i => i.IssuedQty)
                .GreaterThan(0).WithMessage("Issued quantity must be greater than 0.");
        });
    }
}
