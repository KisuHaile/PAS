using Application.Common.Mappings;
using AutoMapper;

namespace Application.Features.Receiving.Inspections.Dtos;

public class InspectionDto : IMapFrom<Domain.Receiving.InspectionLog>
{
    public Guid Id { get; set; }
    public Guid ReceivingNoteId { get; set; }
    public string GRNNumber { get; set; } = string.Empty;
    public Guid InspectorId { get; set; }
    public string InspectorName { get; set; } = string.Empty;
    public bool IsPassed { get; set; }
    public string Status => IsPassed ? "Passed" : "Failed";
    public string DeviationNotes { get; set; } = string.Empty;
    public DateTime InspectionDate { get; set; }
    public int ReceivedQuantity { get; set; }
    public List<InspectionItemDto> Items { get; set; } = new();

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Domain.Receiving.InspectionLog, InspectionDto>()
            .ForMember(d => d.GRNNumber, opt => opt.MapFrom(s => s.ReceivingNote != null ? s.ReceivingNote.GRNNumber : string.Empty))
            .ForMember(d => d.InspectorName, opt => opt.Ignore())
            .ForMember(d => d.Items, opt => opt.Ignore());
    }
}

public class InspectionDetailDto : InspectionDto
{
    public ReceivingNoteSummaryDto ReceivingNoteSummary { get; set; } = new();
    public List<InspectionAttachmentDto> Attachments { get; set; } = new();
    public List<InspectionDeviationDto> Deviations { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class InspectionItemDto
{
    public Guid ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public int ReceivedQuantity { get; set; }
    public int InspectedQuantity { get; set; }
    public int AcceptedQuantity { get; set; }
    public int RejectedQuantity { get; set; }
    public string? Remarks { get; set; }
    public bool IsPassed { get; set; }
}

public class InspectionAttachmentDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime UploadedAt { get; set; }
    public string UploadedBy { get; set; } = string.Empty;
}

public class InspectionDeviationDto
{
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string CorrectiveAction { get; set; } = string.Empty;
}

public class ReceivingNoteSummaryDto
{
    public Guid Id { get; set; }
    public string GRNNumber { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public DateTime ReceivedDate { get; set; }
    public string ReceivedBy { get; set; } = string.Empty;
    public int TotalItems { get; set; }
    public int TotalQuantity { get; set; }
}