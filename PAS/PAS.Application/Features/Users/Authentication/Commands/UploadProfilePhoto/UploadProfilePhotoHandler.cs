using Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Persistence.Identity;

namespace Application.Features.Users.Authentication.Commands;

public class UploadProfilePhotoCommandHandler : IRequestHandler<UploadProfilePhotoCommand, Result<string>>
{
    private readonly IFileStorageService _fileStorage;
    private readonly ICurrentUserService _currentUser;
    private readonly UserManager<ApplicationUser> _userManager;

    public UploadProfilePhotoCommandHandler(
        IFileStorageService fileStorage,
        ICurrentUserService currentUser,
        UserManager<ApplicationUser> userManager)
    {
        _fileStorage = fileStorage;
        _currentUser = currentUser;
        _userManager = userManager;
    }

    public async Task<Result<string>> Handle(UploadProfilePhotoCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId == null)
        {
            return Result<string>.Failure("User not authenticated.");
        }

        if (request.Photo == null || request.Photo.Length == 0)
        {
            return Result<string>.Failure("No file uploaded.");
        }

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var extension = Path.GetExtension(request.Photo.FileName).ToLowerInvariant();
        if (Array.IndexOf(allowedExtensions, extension) < 0)
        {
            return Result<string>.Failure("Invalid file type. Allowed: .jpg, .jpeg, .png, .gif");
        }

        const long maxBytes = 5 * 1024 * 1024;
        if (request.Photo.Length > maxBytes)
        {
            return Result<string>.Failure("File is too large. Max 5MB.");
        }

        var user = await _userManager.FindByIdAsync(_currentUser.UserId.Value.ToString());
        if (user == null)
        {
            return Result<string>.Failure("User not found.");
        }

        byte[] bytes;
        using (var ms = new MemoryStream())
        {
            await request.Photo.CopyToAsync(ms, cancellationToken);
            bytes = ms.ToArray();
        }

        var filePath = await _fileStorage.SaveFileAsync(bytes, request.Photo.FileName, "profile", cancellationToken);
        var url = await _fileStorage.GetFileUrlAsync(filePath);

        user.ProfileImageUrl = url;
        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            var errors = string.Join("; ", updateResult.Errors.Select(e => e.Description));
            return Result<string>.Failure($"Failed to update user photo: {errors}");
        }

        return Result<string>.Success(url);
    }
}
