using Application.Features.Common.AuditTrail.Dtos;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Persistence.Identity;

namespace Application.Features.Common.AuditTrail.Queries.GetAuditTrails;

public class GetAuditTrailsHandler : IRequestHandler<GetAuditTrailsQuery, Result<PaginatedList<AuditTrailListDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetAuditTrailsHandler(
        IApplicationDbContext context,
        IMapper mapper,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
    }

    public async Task<Result<PaginatedList<AuditTrailListDto>>> Handle(GetAuditTrailsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.AuditTrails
            .Where(a => !a.IsDeleted)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.UserName))
        {
            var search = request.UserName.Trim();
            var matchingUserIds = await _userManager.Users
                .Where(u => (u.UserName != null && u.UserName.Contains(search)) ||
                            (u.FullName != null && u.FullName.Contains(search)) ||
                            (u.Email != null && u.Email.Contains(search)))
                .Select(u => u.Id)
                .ToListAsync(cancellationToken);

            var matchingGuids = matchingUserIds
                .Select(id => Guid.TryParse(id, out var g) ? g : (Guid?)null)
                .Where(g => g.HasValue)
                .Select(g => g!.Value)
                .ToList();

            if ("system".Contains(search, StringComparison.OrdinalIgnoreCase) || search.Contains("system", StringComparison.OrdinalIgnoreCase))
            {
                matchingGuids.Add(Guid.Empty);
            }

            if (matchingGuids.Count == 0)
            {
                var pageNum = request.PageNumber < 1 ? 1 : request.PageNumber;
                var pageSz = request.PageSize < 1 ? 20 : request.PageSize;
                var emptyList = new PaginatedList<AuditTrailListDto>(new List<AuditTrailListDto>(), 0, pageNum, pageSz);
                return Result<PaginatedList<AuditTrailListDto>>.Success(emptyList);
            }

            query = query.Where(a => matchingGuids.Contains(a.UserId));
        }

        if (!string.IsNullOrWhiteSpace(request.Action))
        {
            query = query.Where(a => a.Action.Contains(request.Action));
        }

        if (!string.IsNullOrWhiteSpace(request.EntityName))
        {
            query = query.Where(a => a.EntityName.Contains(request.EntityName));
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(a => a.ActionDate >= request.FromDate);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(a => a.ActionDate <= request.ToDate);
        }

        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 20 : request.PageSize;

        var paginatedTrails = await query
            .OrderByDescending(a => a.ActionDate)
            .ProjectTo<AuditTrailListDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(pageNumber, pageSize);

        var userGuids = paginatedTrails.Items
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

            foreach (var item in paginatedTrails.Items)
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
            foreach (var item in paginatedTrails.Items)
            {
                item.UserName = "System";
            }
        }

        return Result<PaginatedList<AuditTrailListDto>>.Success(paginatedTrails);
    }
}