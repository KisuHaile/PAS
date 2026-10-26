using Application.Common.Security;
using Application.Features.Users.Employees.Dtos;
using MediatR;

namespace Application.Features.Users.Employees.Queries;

[Authorize(Permissions = Permissions.Employees.View)]
public record GetEmployeeByIdQuery(Guid Id) : IRequest<Result<EmployeeDetailDto>>;