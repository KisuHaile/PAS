using Application.Common.Mappings;
using AutoMapper;

namespace Application.Features.Storage.InventoryStock.Dtos;

public class InventoryStockDto : IMapFrom<Domain.Storage.InventoryStock>
{
    public Guid Id { get; set; }
    public Guid ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public Guid ShelfId { get; set; }
    public string ShelfLocation { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public int CurrentQuantity { get; set; }
    public int ReservedQuantity { get; set; }
    public int AvailableQuantity => CurrentQuantity - ReservedQuantity;
    public string UnitOfMeasure { get; set; } = string.Empty;
    public string? BatchNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DateTime? LastReceived { get; set; }
    public DateTime? LastIssued { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Domain.Storage.InventoryStock, InventoryStockDto>()
            .ForMember(d => d.ItemName, opt => opt.MapFrom(s => s.Item != null ? s.Item.ItemName : string.Empty))
            .ForMember(d => d.SKU, opt => opt.MapFrom(s => s.Item != null ? s.Item.SKU : string.Empty))
            .ForMember(d => d.UnitOfMeasure, opt => opt.MapFrom(s => s.Item != null ? s.Item.UnitOfMeasure : string.Empty))
            .ForMember(d => d.ShelfLocation, opt => opt.MapFrom(s => s.ShelfLocation != null ? $"{s.ShelfLocation.Aisle}-{s.ShelfLocation.Rack}-{s.ShelfLocation.ShelfNumber}" : string.Empty))
            .ForMember(d => d.WarehouseName, opt => opt.MapFrom(s => s.ShelfLocation != null && s.ShelfLocation.Warehouse != null ? s.ShelfLocation.Warehouse.WarehouseName : string.Empty));
    }
}

public class InventoryStockDetailDto : InventoryStockDto
{
    public List<StockMovementDto> RecentMovements { get; set; } = new();
    public StockStatisticsDto Statistics { get; set; } = new();
}

public class StockMovementDto
{
    public DateTime Date { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public int QuantityChange { get; set; }
    public int BalanceAfter { get; set; }
    public string Reference { get; set; } = string.Empty;
    public string PerformedBy { get; set; } = string.Empty;
}

public class StockStatisticsDto
{
    public int TotalReceived { get; set; }
    public int TotalIssued { get; set; }
    public int TotalAdjusted { get; set; }
    public DateTime FirstReceived { get; set; }
    public DateTime LastUpdated { get; set; }
    public int DaysInStock { get; set; }
    public decimal AverageDailyUsage { get; set; }
    public int EstimatedDaysRemaining { get; set; }
}

public class StockByItemDto
{
    public Guid ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public int TotalQuantity { get; set; }
    public int ReservedQuantity { get; set; }
    public int AvailableQuantity => TotalQuantity - ReservedQuantity;
    public List<StockLocationSummaryDto> Locations { get; set; } = new();
}

public class StockLocationSummaryDto
{
    public Guid ShelfId { get; set; }
    public string ShelfLocation { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public int ReservedQuantity { get; set; }
    public int AvailableQuantity => Quantity - ReservedQuantity;
}