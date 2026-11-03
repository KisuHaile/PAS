using Application.Common.Mappings;
using AutoMapper;

namespace Application.Features.Disposal.Dtos;

public class DisposalRecordDto : IMapFrom<Domain.Disposal.DisposalRecord>
{
    public Guid Id { get; set; }
    public Guid ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public string UnitOfMeasure { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public DateTime DisposalDate { get; set; }
    public Guid DisposedBy { get; set; }
    public string DisposedByName { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public Guid? ApprovedById { get; set; }
    public string? ApprovedByName { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public string? ApprovalRemarks { get; set; }
    public decimal EstimatedValue { get; set; }
    public decimal ActualValue { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Domain.Disposal.DisposalRecord, DisposalRecordDto>()
            .ForMember(d => d.ItemName, opt => opt.MapFrom(s => s.Item != null ? s.Item.ItemName : string.Empty))
            .ForMember(d => d.SKU, opt => opt.MapFrom(s => s.Item != null ? s.Item.SKU : string.Empty))
            .ForMember(d => d.UnitOfMeasure, opt => opt.MapFrom(s => s.Item != null ? s.Item.UnitOfMeasure : string.Empty))
            .ForMember(d => d.DisposedByName, opt => opt.Ignore())
            .ForMember(d => d.Status, opt => opt.Ignore())
            .ForMember(d => d.ApprovedByName, opt => opt.Ignore())
            .ForMember(d => d.EstimatedValue, opt => opt.Ignore())
            .ForMember(d => d.ActualValue, opt => opt.Ignore());
    }
}

public class DisposalRecordDetailDto : DisposalRecordDto
{
    public List<DisposalItemDetailDto> Items { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public List<DisposalAuditDto> AuditHistory { get; set; } = new();
    public DisposalApprovalDto Approval { get; set; } = new();
}

public class DisposalItemDetailDto
{
    public Guid ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public int AvailableStock { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalValue => UnitCost * Quantity;
    public string Reason { get; set; } = string.Empty;
}

public class DisposalAuditDto
{
    public DateTime Date { get; set; }
    public string Action { get; set; } = string.Empty;
    public string PerformedBy { get; set; } = string.Empty;
    public string Remarks { get; set; } = string.Empty;
}

public class DisposalApprovalDto
{
    public Guid? ApprovedById { get; set; }
    public string ApprovedByName { get; set; } = string.Empty;
    public DateTime? ApprovedDate { get; set; }
    public string Remarks { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}