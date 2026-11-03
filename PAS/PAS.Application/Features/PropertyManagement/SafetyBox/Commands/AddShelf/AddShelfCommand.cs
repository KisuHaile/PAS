using MediatR;

namespace Application.Features.PropertyManagement.SafetyBoxes.Commands.AddShelf;

public record AddShelfCommand : IRequest<Result<Guid>>
{
    public Guid SafetyBoxId { get; init; }
    public int ShelfNumber { get; init; }
}
