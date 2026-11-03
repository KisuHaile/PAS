using Application.Common.Mappings;
using AutoMapper;

namespace Application.Features.TransferReturn.TransferRecords.Dtos;

public class TransferRecordDto : IMapFrom<Domain.TransferReturn.TransferRecord>
{
    public Guid Id { get; set; }
    public string TransferNumber { get; set; } = string.Empty;
    public Guid ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string ItemSKU { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public Guid FromLocationId { get; set; }
    public string FromLocationName { get; set; } = string.Empty;
    public string FromLocationType { get; set; } = string.Empty;
    public Guid ToLocationId { get; set; }
    public string ToLocationName { get; set; } = string.Empty;
    public string ToLocationType { get; set; } = string.Empty;
    public Guid? FromShelfId { get; set; }
    public string FromShelfLocation { get; set; } = string.Empty;
    public Guid? ToShelfId { get; set; }
    public string ToShelfLocation { get; set; } = string.Empty;
    public DateTime TransferDate { get; set; }
    public Guid InitiatedById { get; set; }
    public string InitiatedByName { get; set; } = string.Empty;
    public Guid? ApprovedById { get; set; }
    public string? ApprovedByName { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? BatchNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public string? Reference { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Domain.TransferReturn.TransferRecord, TransferRecordDto>()
            .ForMember(d => d.ItemName, opt => opt.MapFrom(s => s.Item != null ? s.Item.ItemName : string.Empty))
            .ForMember(d => d.ItemSKU, opt => opt.MapFrom(s => s.Item != null ? s.Item.SKU : string.Empty))
            .ForMember(d => d.FromLocationName, opt => opt.MapFrom(s => s.FromLocation != null ? s.FromLocation.Name : string.Empty))
            .ForMember(d => d.FromLocationType, opt => opt.MapFrom(s => s.FromLocation != null ? s.FromLocation.LocationType : string.Empty))
            .ForMember(d => d.ToLocationName, opt => opt.MapFrom(s => s.ToLocation != null ? s.ToLocation.Name : string.Empty))
            .ForMember(d => d.ToLocationType, opt => opt.MapFrom(s => s.ToLocation != null ? s.ToLocation.LocationType : string.Empty))
            .ForMember(d => d.FromShelfLocation, opt => opt.MapFrom(s => s.FromShelf != null ? $"{s.FromShelf.Aisle}-{s.FromShelf.Rack}-{s.FromShelf.ShelfNumber}" : string.Empty))
            .ForMember(d => d.ToShelfLocation, opt => opt.MapFrom(s => s.ToShelf != null ? $"{s.ToShelf.Aisle}-{s.ToShelf.Rack}-{s.ToShelf.ShelfNumber}" : string.Empty))
            .ForMember(d => d.InitiatedByName, opt => opt.Ignore())
            .ForMember(d => d.ApprovedByName, opt => opt.Ignore());
    }
}

public class TransferRecordDetailDto : TransferRecordDto
{
    public List<TransferHistoryDto> History { get; set; } = new();
    public TransferDocumentDto? Document { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class TransferHistoryDto
{
    public DateTime Date { get; set; }
    public string Action { get; set; } = string.Empty;
    public string PerformedBy { get; set; } = string.Empty;
    public string? Remarks { get; set; }
}

public class TransferDocumentDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
}

public class TransferListDto
{
    public Guid Id { get; set; }
    public string TransferNumber { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string FromLocation { get; set; } = string.Empty;
    public string ToLocation { get; set; } = string.Empty;
    public DateTime TransferDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string InitiatedBy { get; set; } = string.Empty;
}

public class CreateTransferDto
{
    public Guid ItemId { get; set; }
    public int Quantity { get; set; }
    public Guid ToLocationId { get; set; }
    public Guid? ToShelfId { get; set; }
    public string? BatchNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public string? Reference { get; set; }
}