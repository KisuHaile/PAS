namespace Application.Common.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task SendEmailWithAttachmentAsync(string to, string subject, string body, byte[] attachment, string attachmentName);
        Task SendEmailToMultipleAsync(string[] to, string subject, string body);
    }
}