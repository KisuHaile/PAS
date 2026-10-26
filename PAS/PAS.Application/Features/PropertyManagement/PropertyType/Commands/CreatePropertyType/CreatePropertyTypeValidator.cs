using FluentValidation;

namespace Application.Features.PropertyManagement.PropertyTypes.Commands.CreatePropertyType;

public class CreatePropertyTypeCommandValidator : AbstractValidator<CreatePropertyTypeCommand>
{
    public CreatePropertyTypeCommandValidator()
    {
        RuleFor(v => v.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");

        RuleFor(v => v.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
    }
}