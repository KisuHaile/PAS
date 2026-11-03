using Application.Events;
using MediatR;

namespace Application.Features.PropertyManagement.Properties.Commands.DeleteProperty;

public class DeletePropertyCommandHandler : IRequestHandler<DeletePropertyCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public DeletePropertyCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result> Handle(DeletePropertyCommand request, CancellationToken cancellationToken)
    {
        var property = await _context.Properties
            .Include(p => p.Attachments)
            .FirstOrDefaultAsync(p => p.Id == request.Id && !p.IsDeleted, cancellationToken);

        if (property == null)
        {
            throw new NotFoundException(nameof(Domain.PropertyManagement.Property), request.Id);
        }

        property.SoftDelete();

        if (property.Attachments?.Any() == true)
        {
            foreach (var attachment in property.Attachments.Where(a => !a.IsDeleted))
            {
                attachment.SoftDelete();
            }
        }

        var auditTrail = new AuditTrail(
            _currentUser.UserGuid ?? Guid.Empty,
            "DELETE",
            nameof(Domain.PropertyManagement.Property),
            property.Id);
        _context.AuditTrails.Add(auditTrail);

        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new EntityDeletedEvent<Domain.PropertyManagement.Property>(property, _currentUser.UserGuid), cancellationToken);

        return Result.Success();
    }
}