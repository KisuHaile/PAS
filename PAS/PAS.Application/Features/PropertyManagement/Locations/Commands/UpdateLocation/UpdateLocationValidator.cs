using FluentValidation;

namespace Application.Features.PropertyManagement.Locations.Commands.UpdateLocation;

public class UpdateLocationCommandValidator : AbstractValidator<UpdateLocationCommand>
{
    public UpdateLocationCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty().WithMessage("Id is required.");

        RuleFor(v => v.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(500).WithMessage("Name must not exceed 50 characters.");

        RuleFor(v => v.LocationType)
            .NotEmpty().WithMessage("Location type is required.")
            .MaximumLength(50).WithMessage("Location type must not exceed 50 characters.");
    }
}