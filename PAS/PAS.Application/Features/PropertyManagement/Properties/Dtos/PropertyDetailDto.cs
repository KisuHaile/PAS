namespace Application.Features.PropertyManagement.Properties.Dtos;

public class PropertyDetailDto : PropertyDto
{
    public List<PropertyAttachmentDto> Attachments { get; set; } = new();
    public List<PropertyMovementDto> MovementHistory { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}

public class PropertyAttachmentDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime UploadedAt { get; set; }
}

public class PropertyMovementDto
{
    public DateTime Date { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public string FromLocation { get; set; } = string.Empty;
    public string ToLocation { get; set; } = string.Empty;
    public string PerformedBy { get; set; } = string.Empty;
    public string Remarks { get; set; } = string.Empty;
}