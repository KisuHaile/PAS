using Application.Common.Security;
using Application.Features.Users.Employees.Dtos;
using MediatR;

namespace Application.Features.Users.Employees.Queries;

[Authorize(Permissions = Permissions.Employees.View)]
public record GetEmployeesQuery : IRequest<Result<PaginatedList<EmployeeListDto>>>
{
    public string? SearchTerm { get; init; }
    public string? Department { get; init; }
    public bool? IsActive { get; init; }
    public bool? HasUserAccount { get; init; }
    public bool? WithUserAccountOnly { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}