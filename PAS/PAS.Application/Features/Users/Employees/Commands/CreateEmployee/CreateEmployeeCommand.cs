using Application.Common.Security;
using Application.Events;
using MediatR;

namespace Application.Features.Users.Employees.Commands;

[Authorize(Permissions = Permissions.Employees.Create)]
public record CreateEmployeeCommand : IRequest<Result<Guid>>
{
    public string EmployeeCode { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string Department { get; init; } = string.Empty;
    public string Position { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public DateTime? HireDate { get; init; }
}