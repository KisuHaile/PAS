using FluentValidation;

namespace Application.Features.PropertyManagement.Properties.Commands.UpdateProperty;

public class UpdatePropertyCommandValidator : AbstractValidator<UpdatePropertyCommand>
{
    public UpdatePropertyCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty().WithMessage("Id is required.");

        RuleFor(v => v.TagNumber)
            .NotEmpty().WithMessage("Tag number is required.")
            .MaximumLength(50).WithMessage("Tag number must not exceed 50 characters.");

        RuleFor(v => v.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");

        RuleFor(v => v.SerialNumber)
            .NotEmpty().WithMessage("Serial number is required.")
            .MaximumLength(100).WithMessage("Serial number must not exceed 100 characters.");

        RuleFor(v => v.PropertyTypeId)
            .NotEmpty().WithMessage("Property type is required.");

        RuleFor(v => v.UnitPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Unit price must be greater than or equal to 0.");

        RuleFor(v => v.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than or equal to 10.");

        RuleFor(v => v.PurchaseDate)
            .NotEmpty().WithMessage("Purchase date is required.")
            .LessThanOrEqualTo(DateTime.Now).WithMessage("Purchase date cannot be in the future.");

        RuleFor(v => v.LocationId)
            .NotEmpty().WithMessage("Location is required.");
    }
}