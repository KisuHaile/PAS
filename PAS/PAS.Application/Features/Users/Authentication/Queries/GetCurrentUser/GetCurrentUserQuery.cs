using Application.Common.Interfaces;
using Application.Features.Users.Authentication.Dtos;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Persistence.Identity;

namespace Application.Features.Users.Authentication.Queries;

public record GetCurrentUserQuery : IRequest<Result<CurrentUserDto>>;

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, Result<CurrentUserDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUserService _currentUser;

    public GetCurrentUserQueryHandler(UserManager<ApplicationUser> userManager, ICurrentUserService currentUser)
    {
        _userManager = userManager;
        _currentUser = currentUser;
    }

    public async Task<Result<CurrentUserDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId == null)
        {
            return Result<CurrentUserDto>.Failure("User not authenticated.");
        }

        var user = await _userManager.FindByIdAsync(_currentUser.UserId.Value.ToString());
        if (user == null)
        {
            return Result<CurrentUserDto>.Failure("User not found.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? string.Empty;

        var dto = new CurrentUserDto
        {
            Id = Guid.TryParse(user.Id, out var parsedId) ? parsedId : Guid.Empty,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            Role = role,
            ProfileImageUrl = user.ProfileImageUrl
        };

        return Result<CurrentUserDto>.Success(dto);
    }
}
