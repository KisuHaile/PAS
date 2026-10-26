using FluentValidation;
using System;

namespace Application.Features.Common.DocumentAttachments.Commands.UploadDocument;

public class UploadDocumentCommandValidator : AbstractValidator<UploadDocumentCommand>
{
    private readonly string[] _allowedExtensions = { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".jpg", ".jpeg", ".png", ".txt" };
    private readonly string[] _imageExtensions = { ".jpg", ".jpeg", ".png" };
    private const long MaxFileSize = 10 * 1024 * 1024; // 10MB

    public UploadDocumentCommandValidator()
    {
        RuleFor(v => v.File)
            .NotNull().WithMessage("File is required.");

        RuleFor(v => v.File.Length)
            .LessThanOrEqualTo(MaxFileSize).WithMessage($"File size must not exceed {MaxFileSize / 1024 / 1024}MB.");

        RuleFor(v => v.File.FileName)
            .Must(fileName => _allowedExtensions.Contains(Path.GetExtension(fileName).ToLower()))
            .WithMessage($"File type not allowed. Allowed types: {string.Join(", ", _allowedExtensions)}");

        RuleFor(v => v.RelatedEntityId)
            .NotEmpty().WithMessage("Related entity ID is required.");

        RuleFor(v => v.RelatedEntityName)
            .NotEmpty().WithMessage("Related entity name is required.")
            .MaximumLength(100).WithMessage("Related entity name must not exceed 100 characters.");

        When(v => v.File != null && v.RelatedEntityName.Equals("employee", StringComparison.OrdinalIgnoreCase), () =>
        {
            RuleFor(v => v.File.FileName)
                .Must(fileName => _imageExtensions.Contains(Path.GetExtension(fileName).ToLower()))
                .WithMessage($"Employee photo must be an image: {string.Join(", ", _imageExtensions)}");

            RuleFor(v => v.File.ContentType)
                .Must(contentType => contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Employee photo must be an image file.");
        });
    }
}