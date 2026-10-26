using Application.Common.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PAS.API.Configurations;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;

namespace PAS.API.Middleware;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly JwtSettings _jwtSettings;

    public JwtMiddleware(RequestDelegate next, IOptions<JwtSettings> jwtSettings)
    {
        _next = next;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task InvokeAsync(HttpContext context, ICurrentUserService currentUserService)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (token != null)
            AttachUserToContext(context, token, currentUserService);

        await _next(context);
    }

    private void AttachUserToContext(HttpContext context, string token, ICurrentUserService currentUserService)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;

            // Attach user to context items
            context.Items["UserId"] = userId;

            // Also set the HttpContext.User so ICurrentUserService and Authorize filters can read claims
            var claims = jwtToken.Claims.ToList();
            var identity = new ClaimsIdentity(claims, "jwt");
            context.User = new ClaimsPrincipal(identity);
        }
        catch
        {
            // Token validation failed - do nothing
        }
    }
}