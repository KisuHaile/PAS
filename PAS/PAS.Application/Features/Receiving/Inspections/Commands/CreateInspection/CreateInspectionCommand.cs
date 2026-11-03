using Application.Common.Security;
using Application.Events;
using FluentValidation;
using MediatR;

namespace Application.Features.Receiving.Inspections.Commands.CreateInspection;

[Authorize(Permissions = Permissions.Receiving.Inspect)]
public record CreateInspectionCommand : IRequest<Result<Guid>>
{
    public Guid ReceivingNoteId { get; init; }
    public List<InspectionItemCommand> Items { get; init; } = new();
    public string DeviationNotes { get; init; } = string.Empty;
    public List<InspectionDeviationCommand> Deviations { get; init; } = new();
    public bool IsPassed => Items.All(i => i.IsPassed);
}

public record InspectionItemCommand
{
    public Guid ItemId { get; init; }
    public int ReceivedQuantity { get; init; }
    public int AcceptedQuantity { get; init; }
    public int RejectedQuantity { get; init; }
    public string? Remarks { get; init; }
    public bool IsPassed => RejectedQuantity == 0;
}

public record InspectionDeviationCommand
{
    public string Type { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Severity { get; init; } = string.Empty;
    public string CorrectiveAction { get; init; } = string.Empty;
}

public class CreateInspectionCommandValidator : AbstractValidator<CreateInspectionCommand>
{
    public CreateInspectionCommandValidator()
    {
        RuleFor(v => v.ReceivingNoteId)
            .NotEmpty().WithMessage("Receiving note is required.");

        RuleFor(v => v.Items)
            .NotEmpty().WithMessage("At least one item must be inspected.");

        RuleForEach(v => v.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ItemId)
                .NotEmpty().WithMessage("Item is required.");

            item.RuleFor(i => i.ReceivedQuantity)
                .GreaterThan(0).WithMessage("Received quantity must be greater than 0.");

            item.RuleFor(i => i.AcceptedQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Accepted quantity must be greater than or equal to 0.");

            item.RuleFor(i => i.RejectedQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Rejected quantity must be greater than or equal to 0.");

            item.RuleFor(i => i.AcceptedQuantity + i.RejectedQuantity)
                .Equal(i => i.ReceivedQuantity)
                .WithMessage("Accepted quantity plus rejected quantity must equal received quantity.");
        });

        RuleFor(v => v.DeviationNotes)
            .MaximumLength(1000).WithMessage("Deviation notes must not exceed 1000 characters.");
    }
}