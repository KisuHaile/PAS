namespace Application.Features.Catalog.ItemMasters.Dtos;

public class ItemMasterDetailDto : ItemMasterDto
{
    public List<ItemStockLocationDto> StockLocations { get; set; } = new();
    public List<ItemMovementDto> RecentMovements { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class ItemStockLocationDto
{
    public Guid ShelfId { get; set; }
    public string ShelfLocation { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public int CurrentQuantity { get; set; }
    public int ReservedQuantity { get; set; }
    public int AvailableQuantity { get; set; }
}

public class ItemMovementDto
{
    public DateTime Date { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public int QuantityChange { get; set; }
    public string Reference { get; set; } = string.Empty;
    public string ShelfLocation { get; set; } = string.Empty;
}