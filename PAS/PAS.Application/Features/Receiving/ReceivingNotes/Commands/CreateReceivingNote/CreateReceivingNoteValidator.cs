using FluentValidation;

namespace Application.Features.Receiving.ReceivingNotes.Commands;

public class CreateReceivingNoteCommandValidator : AbstractValidator<CreateReceivingNoteCommand>
{
    public CreateReceivingNoteCommandValidator()
    {
        RuleFor(v => v.GRNNumber)
            .NotEmpty().WithMessage("GRN number is required.")
            .MaximumLength(50).WithMessage("GRN number must not exceed 50 characters.");

        RuleFor(v => v.SupplierId)
            .NotEmpty().WithMessage("Supplier is required.");

        RuleFor(v => v.Items)
            .NotEmpty().WithMessage("At least one item is required.");

        RuleFor(v => v.PONumber)
            .MaximumLength(50).WithMessage("PO number must not exceed 50 characters.");

        RuleFor(v => v.InvoiceNumber)
            .MaximumLength(50).WithMessage("Invoice number must not exceed 50 characters.");

        RuleFor(v => v.DeliveryNoteNumber)
            .MaximumLength(50).WithMessage("Delivery note number must not exceed 50 characters.");

        RuleFor(v => v.VehicleNumber)
            .MaximumLength(50).WithMessage("Vehicle number must not exceed 50 characters.");

        RuleFor(v => v.DriverName)
            .MaximumLength(100).WithMessage("Driver name must not exceed 100 characters.");

        RuleFor(v => v.Remarks)
            .MaximumLength(500).WithMessage("Remarks must not exceed 500 characters.");

        RuleForEach(v => v.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ItemId)
                .NotEmpty().WithMessage("Item ID is required.");

            item.RuleFor(i => i.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0.");

            item.RuleFor(i => i.UnitPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Unit price must be greater than or equal to 0.");
        });
    }
}