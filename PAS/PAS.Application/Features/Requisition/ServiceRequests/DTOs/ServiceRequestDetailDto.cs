using Application.Common.Mappings;
using AutoMapper;

namespace Application.Features.Requisition.ServiceRequests.Dtos;

public class ServiceRequestDetailDto : IMapFrom<Domain.Requisition.ServiceRequest>
{
    public Guid Id { get; set; }
    public string SRNumber { get; set; } = string.Empty;
    public Guid RequesterId { get; set; }
    public string RequesterName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid? ApprovedById { get; set; }
    public string? ApproverName { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public string? Remarks { get; set; }
    public List<ServiceRequestItemDetailDto> Items { get; set; } = new();
    public StoreIssueVoucherReferenceDto? StoreIssueVoucher { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Domain.Requisition.ServiceRequest, ServiceRequestDetailDto>()
            .ForMember(d => d.RequesterName, opt => opt.Ignore())
            .ForMember(d => d.ApproverName, opt => opt.Ignore())
            .ForMember(d => d.Department, opt => opt.Ignore());
    }
}

public class ServiceRequestItemDetailDto
{
    public Guid Id { get; set; }
    public Guid ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public string UnitOfMeasure { get; set; } = string.Empty;
    public int RequestedQty { get; set; }
    public int IssuedQty { get; set; }
    public int AvailableStock { get; set; }
    public bool IsStockAvailable => AvailableStock >= RequestedQty;
    public Guid? ShelfId { get; set; }
    public string? ShelfLocation { get; set; }
    public string? Notes { get; set; }
}

public class StoreIssueVoucherReferenceDto
{
    public Guid Id { get; set; }
    public string SIVNumber { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public string IssuedBy { get; set; } = string.Empty;
}

public class ServiceRequestListDto
{
    public Guid Id { get; set; }
    public string SRNumber { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; }
    public string RequesterName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int ItemCount { get; set; }
    public int TotalQuantity { get; set; }
    public int IssuedQuantity { get; set; }
    public bool HasSIV { get; set; }
}