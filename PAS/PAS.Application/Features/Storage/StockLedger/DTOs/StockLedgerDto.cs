using Application.Common.Mappings;
using AutoMapper;

namespace Application.Features.Storage.StockLedger.Dtos;

public class StockLedgerDto : IMapFrom<Domain.Storage.StockLedger>
{
    public Guid Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public Guid ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public Guid ShelfId { get; set; }
    public string ShelfLocation { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public int QuantityChange { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public Guid ReferenceId { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public string? BatchNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? Reason { get; set; }
    public string? Remarks { get; set; }
    public string PerformedBy { get; set; } = string.Empty;

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Domain.Storage.StockLedger, StockLedgerDto>()
            .ForMember(d => d.ItemName, opt => opt.MapFrom(s => s.Item != null ? s.Item.ItemName : string.Empty))
            .ForMember(d => d.SKU, opt => opt.MapFrom(s => s.Item != null ? s.Item.SKU : string.Empty))
            .ForMember(d => d.ShelfLocation, opt => opt.MapFrom(s => s.Shelf != null ? $"{s.Shelf.Aisle}-{s.Shelf.Rack}-{s.Shelf.ShelfNumber}" : string.Empty))
            .ForMember(d => d.WarehouseName, opt => opt.MapFrom(s => s.Shelf != null && s.Shelf.Warehouse != null ? s.Shelf.Warehouse.WarehouseName : string.Empty))
            .ForMember(d => d.PerformedBy, opt => opt.Ignore());
    }
}

public class StockMovementSummaryDto
{
    public int TotalMovements { get; set; }
    public int TotalIn { get; set; }
    public int TotalOut { get; set; }
    public int TotalAdjustments { get; set; }
    public long QuantityIn { get; set; }
    public long QuantityOut { get; set; }
    public long NetChange { get; set; }
}

public class StockMovementByTypeDto
{
    public string TransactionType { get; set; } = string.Empty;
    public int Count { get; set; }
    public long TotalQuantity { get; set; }
    public List<StockLedgerDto> Movements { get; set; } = new();
}

public class StockMovementByDateDto
{
    public DateTime Date { get; set; }
    public long In { get; set; }
    public long Out { get; set; }
    public long Net { get; set; }
}