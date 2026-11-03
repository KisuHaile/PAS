using Application.Common.Security;
using Application.Events;
using FluentValidation;
using MediatR;

namespace Application.Features.Receiving.Suppliers.Commands.CreateSupplier;

[Authorize(Permissions = Permissions.Suppliers.Create)]
public record CreateSupplierCommand : IRequest<Result<Guid>>
{
    public string SupplierName { get; init; } = string.Empty;
    public string ContactPerson { get; init; } = string.Empty;
    public string TinNumber { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public List<SupplierContactCommand> Contacts { get; init; } = new();
}

public record SupplierContactCommand
{
    public string Name { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Position { get; init; } = string.Empty;
    public bool IsPrimary { get; init; }
}

public class CreateSupplierCommandValidator : AbstractValidator<CreateSupplierCommand>
{
    public CreateSupplierCommandValidator()
    {
        RuleFor(v => v.SupplierName)
            .NotEmpty().WithMessage("Supplier name is required.")
            .MaximumLength(200).WithMessage("Supplier name must not exceed 50 characters.");

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

        RuleFor(v => v.Phone)
            .MaximumLength(20).WithMessage("Phone must not exceed 20 characters.");

        RuleFor(v => v.Address)
            .MaximumLength(500).WithMessage("Address must not exceed 100 characters.");
    }
}