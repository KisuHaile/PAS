using Application.Common.Security;
using Application.Events;
using FluentValidation;
using MediatR;

namespace Application.Features.Disposal.Commands.CreateDisposalRecord;

[Authorize(Permissions = Permissions.Disposal.Create)]
public record CreateDisposalRecordCommand : IRequest<Result<Guid>>
{
    public List<DisposalItemCommand> Items { get; init; } = new();
    public string Reason { get; init; } = string.Empty;
}

public record DisposalItemCommand
{
    public Guid ItemId { get; init; }
    public int Quantity { get; init; }
    public string Reason { get; init; } = string.Empty;
}

public class CreateDisposalRecordCommandValidator : AbstractValidator<CreateDisposalRecordCommand>
{
    public CreateDisposalRecordCommandValidator()
    {
        RuleFor(v => v.Items)
            .NotEmpty().WithMessage("At least one item is required.");

        RuleFor(v => v.Reason)
            .NotEmpty().WithMessage("Reason is required.")
            .MaximumLength(500).WithMessage("Reason must not exceed 500 characters.");

        RuleForEach(v => v.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ItemId)
                .NotEmpty().WithMessage("Item is required.");

            item.RuleFor(i => i.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0.");

            item.RuleFor(i => i.Reason)
                .MaximumLength(500).WithMessage("Reason must not exceed 500 characters.");
        });
    }
}