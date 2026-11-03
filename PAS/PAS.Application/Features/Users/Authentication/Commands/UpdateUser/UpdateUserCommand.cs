using Application.Common.Security;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Persistence.Identity;
using Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Users.Authentication.Commands;

[Authorize(Permissions = Permissions.Users.Edit)]
public record UpdateUserCommand : IRequest<Result>
{
    public Guid Id { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public Guid RoleId { get; init; }
    public bool IsActive { get; init; }
    public string? FullName { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Position { get; init; }
    public string? Department { get; init; }
    public string? EmployeeCode { get; init; }
}

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IApplicationDbContext _context;

    public UpdateUserCommandHandler(
        UserManager<ApplicationUser> userManager, 
        RoleManager<ApplicationRole> roleManager,
        IApplicationDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }

    public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id.ToString());
        if (user == null) return Result.Failure("User not found.");

        var existing = await _userManager.FindByNameAsync(request.Username);
        if (existing != null && existing.Id != user.Id)
            return Result.Failure("Username already exists.");

        user.UserName = request.Username;
        user.Email = request.Email;
        user.IsActive = request.IsActive;

        if (!string.IsNullOrWhiteSpace(request.FullName))
        {
            user.FullName = request.FullName;
        }

        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            user.PhoneNumber = request.PhoneNumber;
        }

        if (!string.IsNullOrWhiteSpace(request.Position))
        {
            user.Position = request.Position;
        }

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            return Result.Failure(string.Join("; ", updateResult.Errors.Select(e => e.Description)));

        // Update corresponding employee record
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => (e.Email == user.Email || e.FullName == user.FullName) && !e.IsDeleted, cancellationToken);

        if (employee != null)
        {
            typeof(Domain.Users.Employee).GetProperty("FullName")?.SetValue(employee, request.FullName ?? user.FullName);
            typeof(Domain.Users.Employee).GetProperty("Department")?.SetValue(employee, request.Department ?? employee.Department);
            typeof(Domain.Users.Employee).GetProperty("EmployeeCode")?.SetValue(employee, request.EmployeeCode ?? employee.EmployeeCode);
            typeof(Domain.Users.Employee).GetProperty("Position")?.SetValue(employee, request.Position ?? employee.Position);
            typeof(Domain.Users.Employee).GetProperty("Email")?.SetValue(employee, request.Email ?? user.Email);
            typeof(Domain.Users.Employee).GetProperty("Phone")?.SetValue(employee, request.PhoneNumber ?? user.PhoneNumber);

            _context.Employees.Update(employee);
        }
        else
        {
            var newEmployee = new Domain.Users.Employee(
                request.EmployeeCode ?? $"EMP-{DateTime.UtcNow.Ticks.ToString()[^10..]}",
                request.FullName ?? user.FullName,
                request.Department ?? "General"
            );

            typeof(Domain.Users.Employee).GetProperty("Position")?.SetValue(newEmployee, request.Position);
            typeof(Domain.Users.Employee).GetProperty("Email")?.SetValue(newEmployee, request.Email ?? user.Email);
            typeof(Domain.Users.Employee).GetProperty("Phone")?.SetValue(newEmployee, request.PhoneNumber);

            _context.Employees.Add(newEmployee);
        }

        await _context.SaveChangesAsync(cancellationToken);

        // Update role if requested and valid RoleId is provided
        if (request.RoleId != Guid.Empty)
        {
            var role = await _roleManager.FindByIdAsync(request.RoleId.ToString());
            if (role != null && !string.IsNullOrWhiteSpace(role.Name))
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                if (currentRoles.Any())
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, role.Name);
            }
        }

        return Result.Success();
    }
}
