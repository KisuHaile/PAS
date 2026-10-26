using Application.Common.Mappings;
using AutoMapper;

namespace Application.Features.Requisition.StoreIssueVouchers.Dtos;

public class StoreIssueVoucherDto : IMapFrom<Domain.Requisition.StoreIssueVoucher>
{
    public Guid Id { get; set; }
    public string SIVNumber { get; set; } = string.Empty;
    public Guid SRId { get; set; }
    public string SRNumber { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public Guid IssuedById { get; set; }
    public string IssuedByName { get; set; } = string.Empty;
    public string RecipientSignature { get; set; } = string.Empty;
    public string? RecipientName { get; set; }
    public string? Remarks { get; set; }
    public string Status { get; set; } = string.Empty;
    public int TotalItems { get; set; }
    public int TotalQuantity { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Domain.Requisition.StoreIssueVoucher, StoreIssueVoucherDto>()
            .ForMember(d => d.SRNumber, opt => opt.MapFrom(s => s.ServiceRequest != null ? s.ServiceRequest.SRNumber : string.Empty))
            .ForMember(d => d.IssuedByName, opt => opt.Ignore())
            .ForMember(d => d.TotalItems, opt => opt.Ignore())
            .ForMember(d => d.TotalQuantity, opt => opt.Ignore());
    }
}

public class StoreIssueVoucherDetailDto : StoreIssueVoucherDto
{
    public List<SIVItemDetailDto> Items { get; set; } = new();
    public ServiceRequestSummaryDto ServiceRequest { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class SIVItemDetailDto
{
    public Guid ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public int RequestedQty { get; set; }
    public int IssuedQty { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    public Guid? ShelfId { get; set; }
    public string ShelfLocation { get; set; } = string.Empty;
    public string? BatchNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
}

public class ServiceRequestSummaryDto
{
    public Guid Id { get; set; }
    public string SRNumber { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; }
    public string RequesterName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
}

public class CreateStoreIssueVoucherDto
{
    public Guid SRId { get; set; }
    public string RecipientSignature { get; set; } = string.Empty;
    public string? RecipientName { get; set; }
    public string? Remarks { get; set; }
    public List<IssueItemDto> Items { get; set; } = new();
}

public class IssueItemDto
{
    public Guid SRDetailId { get; set; }
    public int IssuedQty { get; set; }
    public Guid? ShelfId { get; set; }
    public string? BatchNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
}