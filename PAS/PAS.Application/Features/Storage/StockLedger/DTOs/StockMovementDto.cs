using Application.Common.Mappings;
using AutoMapper;

namespace Application.Features.Storage.StockLedger.Dtos;

public class StockMovementDto : IMapFrom<Domain.Storage.StockLedger>
{
    public Guid Id { get; set; }    
    public DateTime Date { get; set; }
    public Guid ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public Guid ShelfId { get; set; }
    public string ShelfLocation { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public int QuantityChange { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public Guid ReferenceId { get; set; }
    public string? BatchNumber { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Domain.Storage.StockLedger, StockMovementDto>()
            .ForMember(d => d.Date, opt => opt.MapFrom(s => s.CreatedDate))
            .ForMember(d => d.ItemName, opt => opt.MapFrom(s => s.Item != null ? s.Item.ItemName : string.Empty))
            .ForMember(d => d.SKU, opt => opt.MapFrom(s => s.Item != null ? s.Item.SKU : string.Empty))
            .ForMember(d => d.ShelfLocation, opt => opt.MapFrom(s => s.Shelf != null ? $"{s.Shelf.Aisle}-{s.Shelf.Rack}-{s.Shelf.ShelfNumber}" : string.Empty))
            .ForMember(d => d.WarehouseName, opt => opt.MapFrom(s => s.Shelf != null && s.Shelf.Warehouse != null ? s.Shelf.Warehouse.WarehouseName : string.Empty));
    }
}
