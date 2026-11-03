using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistence.Identity;
using System.Net;
using Application.Common.Interfaces;

namespace Application.Features.Users.Authentication.Commands;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result<ForgotPasswordResponseDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly ILogger<ForgotPasswordCommandHandler> _logger;
    private readonly IHostEnvironment _environment;

    public ForgotPasswordCommandHandler(
        UserManager<ApplicationUser> userManager,
        IEmailService emailService,
        ILogger<ForgotPasswordCommandHandler> logger,
        IHostEnvironment environment)
    {
        _userManager = userManager;
        _emailService = emailService;
        _logger = logger;
        _environment = environment;
    }

    public async Task<Result<ForgotPasswordResponseDto>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            return Result<ForgotPasswordResponseDto>.Failure("Email is required.");

        var user = await _userManager.FindByEmailAsync(request.Email.Trim());

        // Do not leak whether the account exists.
        if (user == null || string.IsNullOrWhiteSpace(user.Email))
            return Result<ForgotPasswordResponseDto>.Success(new ForgotPasswordResponseDto { EmailSent = true });

        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = WebUtility.UrlEncode(resetToken);
        var encodedEmail = WebUtility.UrlEncode(user.Email);

        _logger.LogInformation("Password reset token generated for {Email}. Token: {Token}", user.Email, resetToken);

        var message = $@"
<p>Hello {WebUtility.HtmlEncode(user.UserName ?? user.Email)},</p>
<p>You requested a password reset for your PAS account.</p>
<p>Use this token in <strong>/api/Auth/reset-password</strong>:</p>
<p><code>{WebUtility.HtmlEncode(resetToken)}</code></p>
<p>Or use encoded values:</p>
<p>Email: <code>{encodedEmail}</code><br/>Token: <code>{encodedToken}</code></p>
<p>If you did not request this, please ignore this email.</p>";

        try
        {
            await _emailService.SendEmailAsync(user.Email, "PAS Password Reset", message);
            return Result<ForgotPasswordResponseDto>.Success(new ForgotPasswordResponseDto
            {
                EmailSent = true,
                Token = _environment.IsDevelopment() ? resetToken : string.Empty,
                EncodedToken = _environment.IsDevelopment() ? encodedToken : string.Empty
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed sending reset password email for {Email}", user.Email);
            // Keep response generic/success so reset flow can continue in environments
            // where SMTP is not configured (token is logged above for local testing).
            return Result<ForgotPasswordResponseDto>.Success(new ForgotPasswordResponseDto
            {
                EmailSent = false,
                Token = _environment.IsDevelopment() ? resetToken : string.Empty,
                EncodedToken = _environment.IsDevelopment() ? encodedToken : string.Empty
            });
        }
    }
}

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ResetPasswordCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Token) || string.IsNullOrWhiteSpace(request.NewPassword))
            return Result.Failure("Email, token and new password are required.");

        var user = await _userManager.FindByEmailAsync(request.Email.Trim());
        if (user == null)
            return Result.Failure("Invalid reset token.");

        // Accept raw, '+' normalized, and URL-decoded token inputs.
        var candidates = new List<string>
        {
            request.Token.Trim(),
            request.Token.Trim().Replace(" ", "+"),
            WebUtility.UrlDecode(request.Token.Trim())
        }
        .Where(t => !string.IsNullOrWhiteSpace(t))
        .Distinct()
        .ToList();

        IdentityResult? resetResult = null;
        foreach (var token in candidates)
        {
            resetResult = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);
            if (resetResult.Succeeded)
            {
                return Result.Success();
            }
        }

        return Result.Failure(string.Join("; ", resetResult?.Errors.Select(e => e.Description) ?? new[] { "Invalid reset token." }));
    }
}
