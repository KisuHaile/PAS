using Application.Common.Models;
using Application.Features.Users.Employees.Dtos;

namespace Application.Features.Users.Employees.Queries;

public class GetEmployeeByUserIdQueryHandler : IRequestHandler<GetEmployeeByUserIdQuery, Result<EmployeeDetailDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetEmployeeByUserIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<EmployeeDetailDto>> Handle(GetEmployeeByUserIdQuery request, CancellationToken cancellationToken)
    {
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