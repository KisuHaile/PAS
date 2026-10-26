using Application.Common.Security;
using Application.Events;
using FluentValidation;
using MediatR;

namespace Application.Features.Disposal.Commands.ApproveDisposal;

[Authorize(Permissions = Permissions.Disposal.Approve)]
public record ApproveDisposalCommand : IRequest<Result>
{
    public Guid Id { get; init; }
    public bool IsApproved { get; init; }
    public string? Remarks { get; init; }
    public decimal? ActualValue { get; init; }
}

public class ApproveDisposalCommandValidator : AbstractValidator<ApproveDisposalCommand>
{
    public ApproveDisposalCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty().WithMessage("Disposal record ID is required.");

        RuleFor(v => v.ActualValue)
            .GreaterThanOrEqualTo(0).When(v => v.ActualValue.HasValue)
            .WithMessage("Actual value must be greater than or equal to 0.");
    }
}