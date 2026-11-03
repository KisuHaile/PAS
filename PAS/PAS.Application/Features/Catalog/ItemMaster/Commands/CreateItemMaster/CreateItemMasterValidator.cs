using FluentValidation;

namespace Application.Features.Catalog.ItemMasters.Commands.CreateItemMaster;

public class CreateItemMasterCommandValidator : AbstractValidator<CreateItemMasterCommand>
{
    public CreateItemMasterCommandValidator()
    {
        RuleFor(v => v.SKU)
            .NotEmpty().WithMessage("SKU is required.")
            .MaximumLength(50).WithMessage("SKU must not exceed 50 characters.");

        RuleFor(v => v.ItemName)
            .NotEmpty().WithMessage("Item name is required.")
            .MaximumLength(100).WithMessage("Item name must not exceed 100 characters.");

        RuleFor(v => v.CategoryId)
            .NotEmpty().WithMessage("Category is required.");

        RuleFor(v => v.UnitOfMeasure)
            .NotEmpty().WithMessage("Unit of measure is required.")
            .MaximumLength(20).WithMessage("Unit of measure must not exceed 20 characters.");

        RuleFor(v => v.MinStockLevel)
            .GreaterThanOrEqualTo(0).WithMessage("Minimum stock level must be greater than or equal to 10.");
    }
}