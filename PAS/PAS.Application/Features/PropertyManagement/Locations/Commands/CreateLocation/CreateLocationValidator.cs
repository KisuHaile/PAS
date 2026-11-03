using FluentValidation;

namespace Application.Features.PropertyManagement.Locations.Commands.CreateLocation;

public class CreateLocationCommandValidator : AbstractValidator<CreateLocationCommand>
{
    public CreateLocationCommandValidator()
    {
        RuleFor(v => v.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");

        RuleFor(v => v.LocationType)
            .NotEmpty().WithMessage("Location type is required.")
            .MaximumLength(50).WithMessage("Location type must not exceed 50 characters.");
    }
}