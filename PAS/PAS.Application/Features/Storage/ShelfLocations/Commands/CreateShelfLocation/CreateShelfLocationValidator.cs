using FluentValidation;

namespace Application.Features.Storage.ShelfLocations.Commands;

public class CreateShelfLocationCommandValidator : AbstractValidator<CreateShelfLocationCommand>
{
    public CreateShelfLocationCommandValidator()
    {
        RuleFor(v => v.WarehouseId)
            .NotEmpty().WithMessage("Warehouse is required.");

        RuleFor(v => v.Aisle)
            .NotEmpty().WithMessage("Aisle is required.")
            .MaximumLength(50).WithMessage("Aisle must not exceed 50 characters.");

        RuleFor(v => v.Rack)
            .NotEmpty().WithMessage("Rack is required.")
            .MaximumLength(50).WithMessage("Rack must not exceed 50 characters.");

        RuleFor(v => v.ShelfNumber)
            .NotEmpty().WithMessage("Shelf number is required.")
            .MaximumLength(50).WithMessage("Shelf number must not exceed 50 characters.");

        RuleFor(v => v.Zone)
            .MaximumLength(50).WithMessage("Zone must not exceed 50 characters.");

        RuleFor(v => v.BinType)
            .MaximumLength(50).WithMessage("Bin type must not exceed 50 characters.");

        RuleFor(v => v.Length)
            .GreaterThan(0).When(v => v.Length.HasValue)
            .WithMessage("Length must be greater than 0.");

        RuleFor(v => v.Width)
            .GreaterThan(0).When(v => v.Width.HasValue)
            .WithMessage("Width must be greater than 0.");

        RuleFor(v => v.Height)
            .GreaterThan(0).When(v => v.Height.HasValue)
            .WithMessage("Height must be greater than 0.");

        RuleFor(v => v.MaxWeight)
            .GreaterThan(0).When(v => v.MaxWeight.HasValue)
            .WithMessage("Max weight must be greater than 0.");

        RuleFor(v => v.Capacity)
            .GreaterThanOrEqualTo(0).WithMessage("Capacity must be greater than or equal to 0.");
    }
}