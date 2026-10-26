using Application.Features.Users.Authentication.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Persistence.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Features.Users.Authentication.Commands;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResultDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    public RefreshTokenCommandHandler(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<Result<AuthResultDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var userId = request.RefreshToken?.Trim();
        if (string.IsNullOrWhiteSpace(userId))
            return Result<AuthResultDto>.Failure("Invalid refresh token.");

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null || !user.IsActive)
            return Result<AuthResultDto>.Failure("Invalid refresh token.");

        var roles = await _userManager.GetRolesAsync(user);
        var primaryRole = roles.FirstOrDefault() ?? "User";

        var token = GenerateJwt(user, roles);
        var dto = new AuthResultDto
        {
            Succeeded = true,
            Token = token,
            RefreshToken = user.Id,
            ExpiresAt = DateTime.UtcNow.AddHours(8),
            User = new UserInfoDto
            {
                Id = Guid.TryParse(user.Id, out var parsedId) ? parsedId : Guid.Empty,
                Username = user.UserName ?? string.Empty,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                PhotoUrl = user.ProfileImageUrl ?? string.Empty,
                ProfileImageUrl = user.ProfileImageUrl,
                EmployeeCode = string.Empty,
                Department = string.Empty,
                Roles = roles.ToArray(),
                Permissions = GetPermissionsForRole(primaryRole)
            }
        };

        return Result<AuthResultDto>.Success(dto);
    }

    private string GenerateJwt(ApplicationUser user, IEnumerable<string> roles)
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

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "YourSuperSecretKeyForJWTTokenGenerationThatShouldBeAtLeast32BytesLong"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "PAS",
            audience: _configuration["Jwt:Audience"] ?? "PAS.Client",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
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
