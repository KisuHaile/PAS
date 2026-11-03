using MediatR;

namespace Application.Features.Requisition.StoreIssueVouchers.Commands;

public class ApproveStoreIssueVoucherCommandHandler : IRequestHandler<ApproveStoreIssueVoucherCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public ApproveStoreIssueVoucherCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result> Handle(ApproveStoreIssueVoucherCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.UserGuid.HasValue)
        {
            return Result.Failure("User not authenticated.");
        }

        var siv = await _context.StoreIssueVouchers
            .Include(s => s.ServiceRequest)
            .FirstOrDefaultAsync(s => s.Id == request.Id && !s.IsDeleted, cancellationToken);

        if (siv == null)
        {
            throw new NotFoundException(nameof(Domain.Requisition.StoreIssueVoucher), request.Id);
        }

        if (siv.Status != "Pending")
        {
            return Result.Failure($"SIV is already {siv.Status.ToLower()}.");
        }

        // Create a copy for event
        var oldSiv = new Domain.Requisition.StoreIssueVoucher(
            siv.SRId,
            siv.IssuedById,
            siv.RecipientSignature);

        // Update status
        var newStatus = request.IsApproved ? "Approved" : "Rejected";
        typeof(Domain.Requisition.StoreIssueVoucher).GetProperty(nameof(Domain.Requisition.StoreIssueVoucher.Status))
            ?.SetValue(siv, newStatus);

        if (!string.IsNullOrWhiteSpace(request.Remarks))
        {
            typeof(Domain.Requisition.StoreIssueVoucher).GetProperty("ApprovalRemarks")
                ?.SetValue(siv, request.Remarks);
        }

        siv.MarkUpdated();

        // Create audit trail
        var auditTrail = new AuditTrail(
            _currentUser.UserGuid.Value,
            request.IsApproved ? "APPROVE" : "REJECT",
            nameof(Domain.Requisition.StoreIssueVoucher),
            siv.Id);
        _context.AuditTrails.Add(auditTrail);

        await _context.SaveChangesAsync(cancellationToken);

        // Notify issuer
        await NotifyIssuerAsync(siv, request, cancellationToken);

        await _mediator.Publish(new EntityUpdatedEvent<Domain.Requisition.StoreIssueVoucher>(siv, oldSiv, _currentUser.UserGuid), cancellationToken);

        return Result.Success();
    }

    private async Task NotifyIssuerAsync(
        Domain.Requisition.StoreIssueVoucher siv,
        ApproveStoreIssueVoucherCommand request,
        CancellationToken cancellationToken)
    {
        var status = request.IsApproved ? "approved" : "rejected";
        var message = $"Your store issue voucher {siv.SIVNumber} has been {status}.";

        if (!string.IsNullOrWhiteSpace(request.Remarks))
        {
            message += $" Remarks: {request.Remarks}";
        }

        var notification = new Notification(siv.IssuedById, message);
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync(cancellationToken);
    }
}