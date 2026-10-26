using FluentValidation;

namespace Application.Features.PropertyManagement.PropertyTypes.Commands.UpdatePropertyType;

public class UpdatePropertyTypeCommandValidator : AbstractValidator<UpdatePropertyTypeCommand>
{
    public UpdatePropertyTypeCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty().WithMessage("Id is required.");

        RuleFor(v => v.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");

        RuleFor(v => v.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
    }
}