using Application.Features.Users.Employees.Dtos;

namespace Application.Features.Users.Employees.Queries;

public class GetEmployeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdQuery, Result<EmployeeDetailDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetEmployeeByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<EmployeeDetailDto>> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == request.Id && !e.IsDeleted, cancellationToken);

        if (employee == null)
        {
            throw new NotFoundException(nameof(Domain.Users.Employee), request.Id);
        }

        var employeeDto = _mapper.Map<EmployeeDetailDto>(employee);

        var activities = await _context.AuditTrails
            .Where(a => a.UserId == employee.Id)
            .OrderByDescending(a => a.ActionDate)
            .Take(10)
            .Select(a => new EmployeeActivityDto
            {
                Date = a.ActionDate,
                Action = a.Action,
                Entity = a.EntityName,
                Description = $"{a.Action} {a.EntityName}"
            })
            .ToListAsync(cancellationToken);

        employeeDto.RecentActivities = activities;

        return Result<EmployeeDetailDto>.Success(employeeDto);
    }
}