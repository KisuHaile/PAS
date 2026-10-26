using Application.Events;
using AutoMapper;
using MediatR;

namespace Application.Features.PropertyManagement.Properties.Commands.CreateProperty;

public class CreatePropertyCommandHandler : IRequestHandler<CreatePropertyCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public CreatePropertyCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator,
        IMapper mapper)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
        _mapper = mapper;
    }

    public async Task<Result<Guid>> Handle(CreatePropertyCommand request, CancellationToken cancellationToken)
    {
        var existingTag = await _context.Properties
            .AnyAsync(p => p.TagNumber == request.TagNumber && !p.IsDeleted, cancellationToken);

        if (existingTag)
        {
            return Result<Guid>.Failure($"Property with tag number '{request.TagNumber}' already exists.");
        }

        var propertyType = await _context.PropertyTypes
            .FirstOrDefaultAsync(p => p.Id == request.PropertyTypeId && !p.IsDeleted, cancellationToken);

        if (propertyType == null)
        {
            return Result<Guid>.Failure("Property type not found.");
        }

        if (request.PropertyCategoryId.HasValue)
        {
            var category = await _context.PropertyCategories
                .FirstOrDefaultAsync(c => c.Id == request.PropertyCategoryId && !c.IsDeleted, cancellationToken);

            if (category == null)
            {
                return Result<Guid>.Failure("Property category not found.");
            }
        }

        var location = await _context.PropertyLocations
            .FirstOrDefaultAsync(l => l.Id == request.LocationId && !l.IsDeleted, cancellationToken);

        if (location == null)
        {
            return Result<Guid>.Failure("Location not found.");
        }

        if (request.SafetyBoxId.HasValue)
        {
            var safetyBox = await _context.SafetyBoxes
                .Include(s => s.Shelves)
                .FirstOrDefaultAsync(s => s.Id == request.SafetyBoxId && !s.IsDeleted, cancellationToken);

            if (safetyBox == null)
            {
                return Result<Guid>.Failure("Safety box not found.");
            }

            if (safetyBox.LocationId != request.LocationId)
            {
                return Result<Guid>.Failure("Safety box does not belong to the selected location.");
            }

            if (request.ShelfNumber.HasValue)
            {
                var shelf = safetyBox.Shelves?.FirstOrDefault(sh => sh.ShelfNumber == request.ShelfNumber);
                if (shelf == null)
                {
                    return Result<Guid>.Failure($"Shelf number {request.ShelfNumber} not found in safety box.");
                }
            }
        }

        var property = new Domain.PropertyManagement.Property(
            request.TagNumber,
            request.Name,
            request.SerialNumber,
            request.PropertyTypeId,
            request.UnitPrice,
            request.Quantity,
            request.PurchaseDate,
            request.LocationId,
            request.SafetyBoxId);

        if (request.PropertyCategoryId.HasValue)
        {
            typeof(Domain.PropertyManagement.Property).GetProperty(nameof(Domain.PropertyManagement.Property.PropertyCategoryId))?
                .SetValue(property, request.PropertyCategoryId);
        }

        _context.Properties.Add(property);
        await _context.SaveChangesAsync(cancellationToken);

        var auditTrail = new AuditTrail(
            _currentUser.UserGuid ?? Guid.Empty,
            "CREATE",
            nameof(Domain.PropertyManagement.Property),
            property.Id);
        _context.AuditTrails.Add(auditTrail);
        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new EntityCreatedEvent<Domain.PropertyManagement.Property>(property, _currentUser.UserGuid), cancellationToken);

        return Result<Guid>.Success(property.Id);
    }
}