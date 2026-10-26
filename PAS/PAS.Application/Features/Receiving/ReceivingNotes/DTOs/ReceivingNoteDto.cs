using Application.Common.Mappings;
using AutoMapper;

namespace Application.Features.Receiving.ReceivingNotes.Dtos;

public class ReceivingNoteDto : IMapFrom<Domain.Receiving.ReceivingNote>
{
    public Guid Id { get; set; }
    public string GRNNumber { get; set; } = string.Empty;
    public Guid SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public DateTime ReceivedDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid ReceivedById { get; set; }
    public string ReceivedByName { get; set; } = string.Empty;
    public bool HasInspection { get; set; }
    public int TotalItems { get; set; }
    public int TotalQuantity { get; set; }
    public int AcceptedQuantity { get; set; }
    public int RejectedQuantity { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Domain.Receiving.ReceivingNote, ReceivingNoteDto>()
            .ForMember(d => d.SupplierName, opt => opt.MapFrom(s => s.Supplier != null ? s.Supplier.SupplierName : string.Empty))
            .ForMember(d => d.ReceivedByName, opt => opt.Ignore())
            .ForMember(d => d.HasInspection, opt => opt.MapFrom(s => s.InspectionLog != null))
            .ForMember(d => d.TotalItems, opt => opt.Ignore())
            .ForMember(d => d.TotalQuantity, opt => opt.Ignore())
            .ForMember(d => d.AcceptedQuantity, opt => opt.Ignore())
            .ForMember(d => d.RejectedQuantity, opt => opt.Ignore());
    }
}

public class ReceivingNoteDetailDto : ReceivingNoteDto
{
    public InspectionSummaryDto? Inspection { get; set; }
    public List<ReceivingItemDetailDto> Items { get; set; } = new();
    public List<ReceivingNoteAttachmentDto> Attachments { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}

public class ReceivingItemDetailDto
{
    public Guid ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public string UnitOfMeasure { get; set; } = string.Empty;
    public int ReceivedQuantity { get; set; }
    public int AcceptedQuantity { get; set; }
    public int RejectedQuantity { get; set; }
    public int PendingQuantity => ReceivedQuantity - (AcceptedQuantity + RejectedQuantity);
    public string? Remarks { get; set; }
    public bool RequiresInspection { get; set; }
}

public class InspectionSummaryDto
{
    public Guid Id { get; set; }
    public bool IsPassed { get; set; }
    public string DeviationNotes { get; set; } = string.Empty;
    public DateTime InspectionDate { get; set; }
    public string InspectorName { get; set; } = string.Empty;
    public int AcceptedQuantity { get; set; }
    public int RejectedQuantity { get; set; }
}

public class ReceivingNoteAttachmentDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime UploadedAt { get; set; }
}

public class ReceivingNoteListDto
{
    public Guid Id { get; set; }
    public string GRNNumber { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public DateTime ReceivedDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string ReceivedBy { get; set; } = string.Empty;
    public int ItemCount { get; set; }
    public int TotalQuantity { get; set; }
    public bool HasInspection { get; set; }
}