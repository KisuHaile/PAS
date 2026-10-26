using MediatR;

namespace Application.Features.Receiving.ReceivingNotes.Commands;

public class ApproveReceivingNoteCommandHandler : IRequestHandler<ApproveReceivingNoteCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public ApproveReceivingNoteCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result> Handle(ApproveReceivingNoteCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.UserGuid.HasValue)
        {
            return Result.Failure("User not authenticated.");
        }

        var receivingNote = await _context.ReceivingNotes
            .Include(r => r.Items)
            .FirstOrDefaultAsync(r => r.Id == request.Id && !r.IsDeleted, cancellationToken);

        if (receivingNote == null)
        {
            throw new NotFoundException(nameof(Domain.Receiving.ReceivingNote), request.Id);
        }

        if (receivingNote.Status != "PENDING_INSPECTION")
        {
            return Result.Failure($"Receiving note is already {receivingNote.Status.ToLower()}.");
        }

        // Create inspection log
        var inspection = new Domain.Receiving.InspectionLog(
            receivingNote.Id,
            _currentUser.UserGuid.Value,
            request.IsApproved,
            request.Remarks);

        _context.InspectionLogs.Add(inspection);

        // Update receiving note status
        var newStatus = request.IsApproved ? "APPROVED" : "REJECTED";
        typeof(Domain.Receiving.ReceivingNote).GetProperty(nameof(Domain.Receiving.ReceivingNote.Status))
            ?.SetValue(receivingNote, newStatus);
        receivingNote.MarkUpdated();

        // If approved, update inventory
        if (request.IsApproved)
        {
            await UpdateInventoryAsync(receivingNote, cancellationToken);
        }

        // Create audit trail
        var auditTrail = new AuditTrail(
            _currentUser.UserGuid.Value,
            request.IsApproved ? "APPROVE" : "REJECT",
            nameof(Domain.Receiving.ReceivingNote),
            receivingNote.Id);
        _context.AuditTrails.Add(auditTrail);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private async Task UpdateInventoryAsync(Domain.Receiving.ReceivingNote receivingNote, CancellationToken cancellationToken)
    {
        // This would update inventory based on receiving note items
        // You would need to have ReceivingNoteItems entity to know which items were received

        // For now, update stock ledger entries from "RECEIVED_PENDING_INSPECTION" to "RECEIVED"
        var pendingLedgers = await _context.StockLedgers
            .Where(l => l.ReferenceId == receivingNote.Id &&
                       l.TransactionType == "RECEIVED_PENDING_INSPECTION")
            .ToListAsync(cancellationToken);

        foreach (var ledger in pendingLedgers)
        {
            typeof(StockLedger).GetProperty(nameof(StockLedger.TransactionType))
                ?.SetValue(ledger, "RECEIVED");
        }
    }
}