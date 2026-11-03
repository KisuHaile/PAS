using FluentValidation;

namespace Application.Features.PropertyManagement.PropertyCategories.Commands.UpdatePropertyCategory;

public class UpdatePropertyCategoryCommandValidator : AbstractValidator<UpdatePropertyCategoryCommand>
{
    public UpdatePropertyCategoryCommandValidator()
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