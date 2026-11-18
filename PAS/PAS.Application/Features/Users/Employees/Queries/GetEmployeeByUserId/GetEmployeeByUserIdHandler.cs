using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Security;
using Application.Features.Users.Employees.Dtos;

namespace Application.Features.Users.Employees.Queries;

public class GetEmployeeByUserIdQueryHandler : IRequestHandler<GetEmployeeByUserIdQuery, Result<EmployeeDetailDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public GetEmployeeByUserIdQueryHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<Result<EmployeeDetailDto>> Handle(GetEmployeeByUserIdQuery request, CancellationToken cancellationToken)
    {
        var isSelf = _currentUserService.UserId.HasValue && _currentUserService.UserId.Value == request.UserId;
        var hasPermission = _currentUserService.HasPermission(Permissions.Employees.View) ||
                            _currentUserService.IsInRole("Admin") ||
                            _currentUserService.IsInRole("SuperAdmin");

        if (!isSelf && !hasPermission)
        {
            throw new ForbiddenAccessException("You do not have permission to view another user's employee record.");
        }

        var employee = await _context.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == request.UserId && !e.IsDeleted, cancellationToken);

        if (employee == null)
        {
            return Result<EmployeeDetailDto>.Failure($"Employee not found for User ID {request.UserId}");
        }

        var employeeDto = _mapper.Map<EmployeeDetailDto>(employee);

        return Result<EmployeeDetailDto>.Success(employeeDto);
    }
}