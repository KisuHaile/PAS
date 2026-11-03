using Application.Common.Mappings;
using AutoMapper;

namespace Application.Features.TransferReturn.ReturnMaterialRequests.Dtos;

public class ReturnMaterialRequestDto : IMapFrom<Domain.TransferReturn.ReturnMaterialRequestNote>
{
    public Guid Id { get; set; }
    public string ReturnNumber { get; set; } = string.Empty;
    public Guid ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string ItemSKU { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; }
    public Guid RequestedById { get; set; }
    public string RequestedByName { get; set; } = string.Empty;
    public Guid? ApprovedById { get; set; }
    public string? ApprovedByName { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid? SourceLocationId { get; set; }
    public string SourceLocationName { get; set; } = string.Empty;
    public Guid? SourceShelfId { get; set; }
    public string SourceShelfLocation { get; set; } = string.Empty;
    public Guid? SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public string? ReturnType { get; set; }
    public string? BatchNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? Reference { get; set; }
    public string? Remarks { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Domain.TransferReturn.ReturnMaterialRequestNote, ReturnMaterialRequestDto>()
            .ForMember(d => d.ItemName, opt => opt.MapFrom(s => s.Item != null ? s.Item.ItemName : string.Empty))
            .ForMember(d => d.ItemSKU, opt => opt.MapFrom(s => s.Item != null ? s.Item.SKU : string.Empty))
            .ForMember(d => d.RequestedByName, opt => opt.Ignore())
            .ForMember(d => d.ApprovedByName, opt => opt.Ignore())
            .ForMember(d => d.SourceLocationName, opt => opt.MapFrom(s => s.SourceLocation != null ? s.SourceLocation.Name : string.Empty))
            .ForMember(d => d.SourceShelfLocation, opt => opt.MapFrom(s => s.SourceShelf != null ? $"{s.SourceShelf.Aisle}-{s.SourceShelf.Rack}-{s.SourceShelf.ShelfNumber}" : string.Empty))
            .ForMember(d => d.SupplierName, opt => opt.MapFrom(s => s.Supplier != null ? s.Supplier.SupplierName : string.Empty));
    }
}

public class ReturnMaterialRequestDetailDto : ReturnMaterialRequestDto
{
    public List<ReturnHistoryDto> History { get; set; } = new();
    public List<ReturnDocumentDto> Documents { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class ReturnHistoryDto
{
    public DateTime Date { get; set; }
    public string Action { get; set; } = string.Empty;
    public string PerformedBy { get; set; } = string.Empty;
    public string? Remarks { get; set; }
}

public class ReturnDocumentDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
}

public class ReturnListDto
{
    public Guid Id { get; set; }
    public string ReturnNumber { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string RequestedBy { get; set; } = string.Empty;
}

public class CreateReturnRequestDto
{
    public Guid ItemId { get; set; }
    public int Quantity { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string ReturnType { get; set; } = string.Empty;
    public Guid? SourceShelfId { get; set; }
    public string? BatchNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? Reference { get; set; }
    public string? Remarks { get; set; }
}