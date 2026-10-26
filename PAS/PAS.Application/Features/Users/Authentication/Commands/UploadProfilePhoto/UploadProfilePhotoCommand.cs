using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Users.Authentication.Commands;

public record UploadProfilePhotoCommand : IRequest<Result<string>>
{
    public IFormFile Photo { get; init; } = null!;
}
