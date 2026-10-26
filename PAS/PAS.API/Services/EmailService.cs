using Application.Common.Interfaces;
using Microsoft.Extensions.Options;
using PAS.API.Configurations;
using System.Net;
using System.Net.Mail;

namespace PAS.API.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
    {
        _emailSettings = emailSettings.Value;
        _logger = logger;
    }

    public Task SendEmailAsync(string to, string subject, string body)
    {
        return SendCoreAsync(new[] { to }, subject, body, null, null);
    }

    public Task SendEmailWithAttachmentAsync(string to, string subject, string body, byte[] attachment, string attachmentName)
    {
        return SendCoreAsync(new[] { to }, subject, body, attachment, attachmentName);
    }

    public Task SendEmailToMultipleAsync(string[] to, string subject, string body)
    {
        return SendCoreAsync(to, subject, body, null, null);
    }

    private async Task SendCoreAsync(string[] recipients, string subject, string body, byte[]? attachment, string? attachmentName)
    {
        using var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
        {
            EnableSsl = _emailSettings.EnableSsl,
            Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password)
        };

        using var mailMessage = new MailMessage
        {
            From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        foreach (var recipient in recipients)
            mailMessage.To.Add(recipient);

        if (attachment != null && !string.IsNullOrWhiteSpace(attachmentName))
        {
            var stream = new MemoryStream(attachment);
            mailMessage.Attachments.Add(new Attachment(stream, attachmentName));
        }

        await client.SendMailAsync(mailMessage);
        _logger.LogInformation("Email sent to {Recipients}", string.Join(", ", recipients));
    }
}