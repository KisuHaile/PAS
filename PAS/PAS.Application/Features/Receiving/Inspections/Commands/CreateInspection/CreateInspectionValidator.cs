using FluentValidation;

namespace Application.Features.Receiving.Inspections.Commands.CreateInspection;

public class CreateInspectionValidator : AbstractValidator<CreateInspectionCommand>
{
    public CreateInspectionValidator()
    {
        RuleFor(v => v.ReceivingNoteId)
            .NotEmpty().WithMessage("Receiving note is required.");

        RuleFor(v => v.Items)
            .NotEmpty().WithMessage("At least one item must be inspected.");

        RuleForEach(v => v.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ItemId)
                .NotEmpty().WithMessage("Item is required.");

            item.RuleFor(i => i.ReceivedQuantity)
                .GreaterThan(0).WithMessage("Received quantity must be greater than 0.");

            item.RuleFor(i => i.AcceptedQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Accepted quantity must be greater than or equal to 0.");

            item.RuleFor(i => i.RejectedQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Rejected quantity must be greater than or equal to 0.");

            item.RuleFor(i => i.AcceptedQuantity + i.RejectedQuantity)
                .Equal(i => i.ReceivedQuantity)
                .WithMessage("Accepted quantity plus rejected quantity must equal received quantity.");
        });

        RuleFor(v => v.DeviationNotes)
            .MaximumLength(1000).WithMessage("Deviation notes must not exceed 1000 characters.");
    }
}
