using FluentValidation;

namespace Application.Features.PropertyManagement.SafetyBoxes.Commands.UpdateSafetyBox;

public class UpdateSafetyBoxCommandValidator : AbstractValidator<UpdateSafetyBoxCommand>
{
    public UpdateSafetyBoxCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty().WithMessage("Id is required.");

        RuleFor(v => v.BoxNumber)
            .NotEmpty().WithMessage("Box number is required.")
            .MaximumLength(50).WithMessage("Box number must not exceed 50 characters.");

        RuleFor(v => v.TotalShelves)
            .GreaterThan(0).WithMessage("Total shelves must be greater than 0.")
            .LessThanOrEqualTo(100).WithMessage("Total shelves must not exceed 100.");

        RuleFor(v => v.LocationId)
            .NotEmpty().WithMessage("Location is required.");
    }
}