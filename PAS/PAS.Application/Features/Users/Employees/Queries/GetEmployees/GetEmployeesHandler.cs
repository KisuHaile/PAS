using Application.Features.Users.Employees.Dtos;

namespace Application.Features.Users.Employees.Queries;

public class GetEmployeesQueryHandler : IRequestHandler<GetEmployeesQuery, Result<PaginatedList<EmployeeListDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetEmployeesQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedList<EmployeeListDto>>> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Employees
            .Where(e => !e.IsDeleted)
            .AsNoTracking();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(e =>
                e.EmployeeCode.Contains(request.SearchTerm) ||
                e.FullName.Contains(request.SearchTerm) ||
                (e.Email != null && e.Email.Contains(request.SearchTerm)));
        }

        if (!string.IsNullOrWhiteSpace(request.Department))
        {
            query = query.Where(e => e.Department == request.Department);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(e => e.IsActive == request.IsActive);
        }

        if (request.HasUserAccount.HasValue)
        {
            if (request.HasUserAccount.Value)
                query = query.Where(e => false);
            else
                query = query.Where(e => true);
        }

        // Project to DTO
        var projectedQuery = query.Select(e => new EmployeeListDto
        {
            Id = e.Id,
            EmployeeCode = e.EmployeeCode,
            FullName = e.FullName,
            Department = e.Department,
            Position = e.Position ?? string.Empty,
            Email = e.Email ?? string.Empty,
            IsActive = e.IsActive,
            HasUserAccount = false
        });

        var paginatedEmployees = await projectedQuery
            .OrderBy(e => e.FullName)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return Result<PaginatedList<EmployeeListDto>>.Success(paginatedEmployees);
    }
}