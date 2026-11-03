using FluentValidation;

namespace Application.Features.Users.Employees.Commands;

public class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
{
    public CreateEmployeeCommandValidator()
    {
        RuleFor(v => v.EmployeeCode)
            .NotEmpty().WithMessage("Employee code is required.")
            .MaximumLength(20).WithMessage("Employee code must not exceed 20 characters.");

        RuleFor(v => v.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MaximumLength(100).WithMessage("Full name must not exceed 100 characters.");

        RuleFor(v => v.Department)
            .NotEmpty().WithMessage("Department is required.")
            .MaximumLength(50).WithMessage("Department must not exceed 50 characters.");

        RuleFor(v => v.Position)
            .MaximumLength(100).WithMessage("Position must not exceed 100 characters.");

        RuleFor(v => v.Email)
            .EmailAddress().WithMessage("Valid email is required.")
            .MaximumLength(100).WithMessage("Email must not exceed 100 characters.")
            .When(v => !string.IsNullOrWhiteSpace(v.Email));

        RuleFor(v => v.Phone)
            .MaximumLength(20).WithMessage("Phone must not exceed 20 characters.")
            .When(v => !string.IsNullOrWhiteSpace(v.Phone));

        RuleFor(v => v.HireDate)
            .LessThanOrEqualTo(DateTime.Now).WithMessage("Hire date cannot be in the future.")
            .When(v => v.HireDate.HasValue);
    }
}
