using Application.Common.Security;
using Application.Events;
using MediatR;

namespace Application.Features.Users.Employees.Commands;

[Authorize(Permissions = Permissions.Employees.Delete)]
public record DeleteEmployeeCommand(Guid Id) : IRequest<Result>;