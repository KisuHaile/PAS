using FluentValidation;

namespace Application.Features.PropertyManagement.PropertyCategories.Commands.CreatePropertyCategory;

public class CreatePropertyCategoryCommandValidator : AbstractValidator<CreatePropertyCategoryCommand>
{
    public CreatePropertyCategoryCommandValidator()
    {
        RuleFor(v => v.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");

        RuleFor(v => v.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
    }
}