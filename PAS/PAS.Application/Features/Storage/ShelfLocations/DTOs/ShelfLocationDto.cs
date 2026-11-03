using Application.Common.Mappings;
using AutoMapper;

namespace Application.Features.Storage.ShelfLocations.Dtos;

public class ShelfLocationDto : IMapFrom<Domain.Storage.ShelfLocation>
{
    public Guid Id { get; set; }
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public string Aisle { get; set; } = string.Empty;
    public string Rack { get; set; } = string.Empty;
    public string ShelfNumber { get; set; } = string.Empty;
    public string QRCodeValue { get; set; } = string.Empty;
    public string FullAddress { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;
    public string BinType { get; set; } = string.Empty;
    public decimal? Length { get; set; }
    public decimal? Width { get; set; }
    public decimal? Height { get; set; }
    public decimal? MaxWeight { get; set; }
    public bool IsActive { get; set; }
    public int CurrentItemCount { get; set; }
    public int CurrentQuantity { get; set; }
    public int Capacity { get; set; }
    public double UtilizationPercentage { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Domain.Storage.ShelfLocation, ShelfLocationDto>()
            .ForMember(d => d.WarehouseName, opt => opt.MapFrom(s => s.Warehouse != null ? s.Warehouse.WarehouseName : string.Empty))
            .ForMember(d => d.FullAddress, opt => opt.MapFrom(s => $"{s.Aisle}-{s.Rack}-{s.ShelfNumber}"))
            .ForMember(d => d.CurrentItemCount, opt => opt.Ignore())
            .ForMember(d => d.CurrentQuantity, opt => opt.Ignore())
            .ForMember(d => d.UtilizationPercentage, opt => opt.Ignore());
    }
}

public class ShelfLocationDetailDto : ShelfLocationDto
{
    public List<ShelfInventoryItemDto> Inventory { get; set; } = new();
    public List<ShelfMovementDto> RecentMovements { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class ShelfInventoryItemDto
{
    public Guid InventoryId { get; set; }
    public Guid ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public int CurrentQuantity { get; set; }
    public int ReservedQuantity { get; set; }
    public int AvailableQuantity => CurrentQuantity - ReservedQuantity;
    public string UnitOfMeasure { get; set; } = string.Empty;
    public string? BatchNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DateTime? LastUpdated { get; set; }
}

public class ShelfMovementDto
{
    public DateTime Date { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public int QuantityChange { get; set; }
    public string Reference { get; set; } = string.Empty;
}

public class ShelfLocationListDto
{
    public Guid Id { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public string FullAddress { get; set; } = string.Empty;
    public string QRCodeValue { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int ItemCount { get; set; }
    public int TotalQuantity { get; set; }
    public int Capacity { get; set; }
}