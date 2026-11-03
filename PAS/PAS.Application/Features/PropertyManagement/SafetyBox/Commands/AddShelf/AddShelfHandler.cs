using MediatR;

namespace Application.Features.PropertyManagement.SafetyBoxes.Commands.AddShelf;

public class AddShelfCommandHandler : IRequestHandler<AddShelfCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public AddShelfCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<Guid>> Handle(AddShelfCommand request, CancellationToken cancellationToken)
    {
        var safetyBox = await _context.SafetyBoxes
            .Include(s => s.Shelves)
            .FirstOrDefaultAsync(s => s.Id == request.SafetyBoxId && !s.IsDeleted, cancellationToken);

        if (safetyBox == null)
        {
            return Result<Guid>.Failure("Safety box not found.");
        }

        var existingShelf = safetyBox.Shelves?.Any(s => s.ShelfNumber == request.ShelfNumber && !s.IsDeleted) == true;
        if (existingShelf)
        {
            return Result<Guid>.Failure($"Shelf number {request.ShelfNumber} already exists.");
        }

        var shelf = new SafetyBoxShelf(request.SafetyBoxId, request.ShelfNumber);
        _context.SafetyBoxShelves.Add(shelf);

        typeof(SafetyBox).GetProperty(nameof(SafetyBox.TotalShelves))?.SetValue(safetyBox, safetyBox.TotalShelves + 1);
        safetyBox.MarkUpdated();

        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(shelf.Id);
    }
}