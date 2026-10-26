using Application.Features.Users.Authentication.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Persistence.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users.Authentication.Commands;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, Result<AuthResultDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<LoginUserCommandHandler> _logger;
    private readonly IApplicationDbContext _context;

    public LoginUserCommandHandler(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration,
        ILogger<LoginUserCommandHandler> logger,
        IApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _logger = logger;
        _context = context;
    }

    public async Task<Result<AuthResultDto>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var loginInput = request.Username?.Trim();
        if (string.IsNullOrWhiteSpace(loginInput) || string.IsNullOrWhiteSpace(request.Password))
        {
            return Result<AuthResultDto>.Failure("Username and password are required.");
        }

        var user = await _userManager.FindByNameAsync(loginInput)
                   ?? await _userManager.FindByEmailAsync(loginInput);

        if (user == null)
        {
            _logger.LogWarning("Login attempt with non-existent username/email: {Username}", loginInput);
            return Result<AuthResultDto>.Failure("Invalid username or password.");
        }

        if (!user.IsActive || !await _signInManager.CanSignInAsync(user))
        {
            _logger.LogWarning("Login blocked for user: {Username}", user.UserName);
            return Result<AuthResultDto>.Failure("User account is not allowed to sign in.");
        }

        var signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
        if (signInResult.IsLockedOut)
        {
            _logger.LogWarning("Locked out login attempt for user: {Username}", user.UserName);
            return Result<AuthResultDto>.Failure("User account is locked. Try again later.");
        }

        if (!signInResult.Succeeded)
        {
            _logger.LogWarning("Invalid password attempt for user: {Username}", user.UserName);
            return Result<AuthResultDto>.Failure("Invalid username or password.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var primaryRole = roles.FirstOrDefault() ?? "User";

        var token = GenerateJwtToken(user, roles);
        var refreshToken = GenerateRefreshToken(user);
        var expiresAt = DateTime.UtcNow.AddHours(request.RememberMe ? 168 : 8);

        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => (e.Email == user.Email || e.FullName == user.FullName) && !e.IsDeleted, cancellationToken);

        var result = new AuthResultDto
        {
            Succeeded = true,
            Token = token,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt,
            User = new UserInfoDto
            {
                Id = Guid.TryParse(user.Id, out var parsedId) ? parsedId : Guid.Empty,
                Username = user.UserName ?? string.Empty,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                PhotoUrl = user.ProfileImageUrl ?? string.Empty,
                ProfileImageUrl = user.ProfileImageUrl ?? string.Empty,
                EmployeeCode = employee?.EmployeeCode ?? string.Empty,
                Department = employee?.Department ?? string.Empty,
                Phone = user.PhoneNumber ?? string.Empty,
                Position = user.Position ?? employee?.Position ?? string.Empty,
                JoinDate = employee != null ? employee.CreatedAt.ToString("yyyy-MM-dd") : DateTime.UtcNow.ToString("yyyy-MM-dd"),
                Roles = roles.ToArray(),
                Permissions = GetPermissionsForRole(primaryRole)
            }
        };

        return Result<AuthResultDto>.Success(result);
    }

    private string GenerateJwtToken(ApplicationUser user, IEnumerable<string> roles)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName ?? string.Empty),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration["Jwt:Key"] ?? "YourSuperSecretKeyForJWTTokenGenerationThatShouldBeAtLeast32BytesLong"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddHours(8);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "PropertyManagementSystem",
            audience: _configuration["Jwt:Audience"] ?? "PropertyManagementClient",
            claims: claims,
            expires: expires,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken(ApplicationUser user)
    {
        return user.Id;
    }

    private string[] GetPermissionsForRole(string role)
    {
        return role switch
        {
            SystemRoles.Admin => SystemRoles.RolePermissions.AdminPermissions,
            SystemRoles.StoreOfficer => SystemRoles.RolePermissions.StoreOfficerPermissions,
            SystemRoles.Staff => SystemRoles.RolePermissions.StaffPermissions,
            SystemRoles.Inspector => SystemRoles.RolePermissions.InspectorPermissions,
            SystemRoles.Approver => SystemRoles.RolePermissions.ApproverPermissions,
            SystemRoles.Manager => SystemRoles.RolePermissions.ManagerPermissions,
            _ => Array.Empty<string>()
        };
    }
}