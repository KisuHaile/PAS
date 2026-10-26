using Application.Common.Security;
using Application.Events;
using FluentValidation;
using MediatR;

namespace Application.Features.Receiving.Suppliers.Commands.UpdateSupplier;

[Authorize(Permissions = Permissions.Suppliers.Edit)]
public record UpdateSupplierCommand : IRequest<Result>
{
    public Guid Id { get; init; }
    public string SupplierName { get; init; } = string.Empty;
    public string ContactPerson { get; init; } = string.Empty;
    public string TinNumber { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public bool IsActive { get; init; } = true;
}

public class UpdateSupplierCommandValidator : AbstractValidator<UpdateSupplierCommand>
{
    public UpdateSupplierCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty().WithMessage("Supplier ID is required.");

        RuleFor(v => v.SupplierName)
            .NotEmpty().WithMessage("Supplier name is required.")
            .MaximumLength(200).WithMessage("Supplier name must not exceed 200 characters.");

        RuleFor(v => v.ContactPerson)
            .NotEmpty().WithMessage("Contact person is required.")
            .MaximumLength(100).WithMessage("Contact person must not exceed 100 characters.");

        RuleFor(v => v.TinNumber)
            .NotEmpty().WithMessage("TIN number is required.")
            .MaximumLength(50).WithMessage("TIN number must not exceed 50 characters.");

        RuleFor(v => v.Email)
            .EmailAddress().When(v => !string.IsNullOrWhiteSpace(v.Email))
            .WithMessage("Invalid email address.")
            .MaximumLength(100).WithMessage("Email must not exceed 100 characters.");
    }
}