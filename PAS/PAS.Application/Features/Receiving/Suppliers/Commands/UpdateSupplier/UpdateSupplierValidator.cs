using FluentValidation;

namespace Application.Features.Receiving.Suppliers.Commands.UpdateSupplier;

public class UpdateSupplierValidator : AbstractValidator<UpdateSupplierCommand>
{
    public UpdateSupplierValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty().WithMessage("Supplier ID is required.");

        RuleFor(v => v.SupplierName)
            .NotEmpty().WithMessage("Supplier name is required.")
            .MaximumLength(200).WithMessage("Supplier name must not exceed 200 characters.");

        RuleFor(v => v.ContactPerson)
            .NotEmpty().WithMessage("Contact person is required.")
            .MaximumLength(100).WithMessage("Contact person must not exceed 100 characters.");

        RuleFor(v => v.TinNumber)
            .NotEmpty().WithMessage("TIN number is required.")
            .MaximumLength(50).WithMessage("TIN number must not exceed 50 characters.");

        RuleFor(v => v.Email)
            .EmailAddress().When(v => !string.IsNullOrWhiteSpace(v.Email))
            .WithMessage("Invalid email address.")
            .MaximumLength(100).WithMessage("Email must not exceed 100 characters.");

        RuleFor(v => v.Phone)
            .MaximumLength(20).WithMessage("Phone must not exceed 20 characters.");

        RuleFor(v => v.Address)
            .MaximumLength(500).WithMessage("Address must not exceed 500 characters.");
    }
}
