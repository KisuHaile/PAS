using Application.Common.Security;
using MediatR;

namespace Application.Features.Requisition.ServiceRequests.Commands.IssueServiceRequest;

[Authorize(Permissions = Permissions.Requisitions.Issue)]
public record IssueServiceRequestCommand : IRequest<Result<Guid>>
{
    public Guid Id { get; init; }
}

public class IssueServiceRequestCommandHandler : IRequestHandler<IssueServiceRequestCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public IssueServiceRequestCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<Guid>> Handle(IssueServiceRequestCommand request, CancellationToken cancellationToken)
    {
        var sr = await _context.ServiceRequests.Include(s => s.Details)
            .FirstOrDefaultAsync(s => s.Id == request.Id && !s.IsDeleted, cancellationToken);
        if (sr == null) return Result<Guid>.Failure("Service request not found.");

        if (sr.Status != "Approved") return Result<Guid>.Failure("Service request must be approved before issuing.");

        var siv = new Domain.Requisition.StoreIssueVoucher(sr.Id, _currentUser.UserId ?? Guid.Empty, "AutoIssued");
        var count = await _context.StoreIssueVouchers.CountAsync(cancellationToken) + 1;
        typeof(Domain.Requisition.StoreIssueVoucher).GetProperty(nameof(Domain.Requisition.StoreIssueVoucher.SIVNumber))?.SetValue(siv, $"SIV-{DateTime.UtcNow:yyyyMMdd}-{count:D4}");
        _context.StoreIssueVouchers.Add(siv);

        foreach (var d in sr.Details)
            d.Issue(d.RequestedQty);

        typeof(Domain.Requisition.ServiceRequest).GetProperty(nameof(Domain.Requisition.ServiceRequest.Status))?.SetValue(sr, "Issued");
        sr.MarkUpdated();

        await _context.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(siv.Id);
    }
}
