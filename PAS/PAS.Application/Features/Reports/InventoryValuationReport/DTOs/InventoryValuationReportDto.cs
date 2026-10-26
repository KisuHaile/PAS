namespace Application.Features.Reports.InventoryValuationReport.Dtos;

public class InventoryValuationReportDto
{
    public DateTime GeneratedAt { get; set; }
    public string GeneratedBy { get; set; } = string.Empty;
    public ReportFilterInfo Filters { get; set; } = new();
    public ReportSummary Summary { get; set; } = new();
    public List<InventoryValuationItemDto> Items { get; set; } = new();
    public List<ValuationByCategoryDto> ByCategory { get; set; } = new();
    public List<ValuationByWarehouseDto> ByWarehouse { get; set; } = new();
}

public class ReportFilterInfo
{
    public DateTime? AsOfDate { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? WarehouseId { get; set; }
    public bool IncludeZeroStock { get; set; }
    public string? ItemSearch { get; set; }
}

public class ReportSummary
{
    public int TotalItems { get; set; }
    public int TotalStockItems { get; set; }
    public long TotalQuantity { get; set; }
    public decimal TotalValue { get; set; }
    public decimal AverageItemValue { get; set; }
    public int LowStockItems { get; set; }
    public int OutOfStockItems { get; set; }
}

public class InventoryValuationItemDto
{
    public Guid ItemId { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string UnitOfMeasure { get; set; } = string.Empty;
    public int CurrentQuantity { get; set; }
    public int ReservedQuantity { get; set; }
    public int AvailableQuantity => CurrentQuantity - ReservedQuantity;
    public decimal AverageCost { get; set; }
    public decimal TotalValue => CurrentQuantity * AverageCost;
    public int MinStockLevel { get; set; }
    public bool IsLowStock => CurrentQuantity <= MinStockLevel;
    public string Status => CurrentQuantity <= 0 ? "Out of Stock" :
                            CurrentQuantity <= MinStockLevel ? "Low Stock" : "In Stock";
    public List<InventoryLocationDto> Locations { get; set; } = new();
}

public class InventoryLocationDto
{
    public string WarehouseName { get; set; } = string.Empty;
    public string ShelfLocation { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Value { get; set; }
}

public class ValuationByCategoryDto
{
    public string CategoryName { get; set; } = string.Empty;
    public int ItemCount { get; set; }
    public long TotalQuantity { get; set; }
    public decimal TotalValue { get; set; }
    public decimal PercentageOfTotal { get; set; }
}

public class ValuationByWarehouseDto
{
    public string WarehouseName { get; set; } = string.Empty;
    public int ItemCount { get; set; }
    public long TotalQuantity { get; set; }
    public decimal TotalValue { get; set; }
    public decimal PercentageOfTotal { get; set; }
}