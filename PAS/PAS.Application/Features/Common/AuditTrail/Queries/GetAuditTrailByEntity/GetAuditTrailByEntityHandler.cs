using Application.Features.Common.AuditTrail.Dtos;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Persistence.Identity;

namespace Application.Features.Common.AuditTrail.Queries.GetAuditTrailByEntity;

public class GetAuditTrailByEntityHandler : IRequestHandler<GetAuditTrailByEntityQuery, Result<List<AuditTrailDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetAuditTrailByEntityHandler(
        IApplicationDbContext context,
        IMapper mapper,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
    }

    public async Task<Result<List<AuditTrailDto>>> Handle(GetAuditTrailByEntityQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.EntityName) || request.EntityId == Guid.Empty)
        {
            return Result<List<AuditTrailDto>>.Success(new List<AuditTrailDto>());
        }

        var trails = await _context.AuditTrails
            .Where(a => a.EntityName == request.EntityName &&
                       a.EntityId == request.EntityId &&
                       !a.IsDeleted)
            .OrderByDescending(a => a.ActionDate)
            .ProjectTo<AuditTrailDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        var userGuids = trails
            .Select(i => i.UserId)
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToList();

        if (userGuids.Any())
        {
            var userIds = userGuids.Select(id => id.ToString()).ToList();
            var users = await _userManager.Users
                .Where(u => userIds.Contains(u.Id))
                .Select(u => new { u.Id, u.UserName, u.FullName })
                .ToListAsync(cancellationToken);

            var userMap = users.ToDictionary(
                u => u.Id,
                u => !string.IsNullOrWhiteSpace(u.UserName) ? u.UserName : (!string.IsNullOrWhiteSpace(u.FullName) ? u.FullName : "System"),
                StringComparer.OrdinalIgnoreCase);

            foreach (var item in trails)
            {
                if (item.UserId == Guid.Empty)
                {
                    item.UserName = "System";
                }
                else if (userMap.TryGetValue(item.UserId.ToString(), out var name))
                {
                    item.UserName = name;
                }
                else
                {
                    item.UserName = "Unknown";
                }
            }
        }
        else
        {
            foreach (var item in trails)
            {
                item.UserName = "System";
            }
        }

        return Result<List<AuditTrailDto>>.Success(trails);
    }
}