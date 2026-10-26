using FluentValidation;

namespace Application.Features.Storage.Warehouses.Commands;

public class CreateWarehouseCommandValidator : AbstractValidator<CreateWarehouseCommand>
{
    public CreateWarehouseCommandValidator()
    {
        RuleFor(v => v.WarehouseName)
            .NotEmpty().WithMessage("Warehouse name is required.")
            .MaximumLength(100).WithMessage("Warehouse name must not exceed 100 characters.");

        RuleFor(v => v.LocationCode)
            .NotEmpty().WithMessage("Location code is required.")
            .MaximumLength(50).WithMessage("Location code must not exceed 50 characters.");

        RuleFor(v => v.Address)
            .MaximumLength(200).WithMessage("Address must not exceed 200 characters.");

        RuleFor(v => v.City)
            .MaximumLength(100).WithMessage("City must not exceed 100 characters.");

        RuleFor(v => v.Country)
            .MaximumLength(100).WithMessage("Country must not exceed 100 characters.");

        RuleFor(v => v.ContactPerson)
            .MaximumLength(100).WithMessage("Contact person must not exceed 100 characters.");

        RuleFor(v => v.ContactPhone)
            .MaximumLength(20).WithMessage("Contact phone must not exceed 20 characters.");

        RuleFor(v => v.ContactEmail)
            .EmailAddress().WithMessage("Valid email is required.")
            .MaximumLength(100).WithMessage("Contact email must not exceed 100 characters.")
            .When(v => !string.IsNullOrWhiteSpace(v.ContactEmail));
    }
}