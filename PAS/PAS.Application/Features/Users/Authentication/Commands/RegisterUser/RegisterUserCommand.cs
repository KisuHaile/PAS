using FluentValidation;
using MediatR;

namespace Application.Features.Users.Authentication.Commands;

public record RegisterUserCommand : IRequest<Result<Guid>>
{
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string Department { get; init; } = string.Empty;
    public string? EmployeeCode { get; init; }
    public string? PhoneNumber { get; init; }
    public string? RoleName { get; init; }
}

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(v => v.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters.")
            .MaximumLength(50).WithMessage("Username must not exceed 50 characters.")
            .Matches("^[a-zA-Z0-9._-]+$").WithMessage("Username can only contain letters, numbers, dots, underscores and hyphens.");

        RuleFor(v => v.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one number.")
            .Matches(@"[!@#$%^&*()_\-+=\[{\]};:<>|./?`~\\]").WithMessage("Password must contain at least one special character.");

        RuleFor(v => v.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Valid email is required.")
            .MaximumLength(100).WithMessage("Email must not exceed 100 characters.");

        RuleFor(v => v.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MaximumLength(100).WithMessage("Full name must not exceed 100 characters.");

        RuleFor(v => v.Department)
            .NotEmpty().WithMessage("Department is required.")
            .MaximumLength(50).WithMessage("Department must not exceed 50 characters.");

        RuleFor(v => v.EmployeeCode)
            .MaximumLength(20).WithMessage("Employee code must not exceed 20 characters.")
            .When(v => !string.IsNullOrWhiteSpace(v.EmployeeCode));

        RuleFor(v => v.RoleName)
            .MaximumLength(50).WithMessage("Role name must not exceed 50 characters.")
            .When(v => !string.IsNullOrWhiteSpace(v.RoleName));
    }
}