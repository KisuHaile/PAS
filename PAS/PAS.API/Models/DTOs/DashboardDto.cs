namespace PAS.API.Models.DTOs;

public class DashboardDto
{
    // Summary Statistics
    public SummaryStatisticsDto Summary { get; set; } = new();

    // Requisition Statistics
    public RequisitionStatisticsDto Requisitions { get; set; } = new();

    // Receiving Statistics
    public ReceivingStatisticsDto Receiving { get; set; } = new();

    // Stock Statistics
    public StockStatisticsDto Stock { get; set; } = new();

    // Property Statistics
    public PropertyStatisticsDto Properties { get; set; } = new();

    // Charts Data
    public ChartDataDto Charts { get; set; } = new();

    // Recent Activities
    public List<RecentActivityDto> RecentActivities { get; set; } = new();

    // Alerts
    public AlertsDto Alerts { get; set; } = new();

    // Quick Actions
    public List<QuickActionDto> QuickActions { get; set; } = new();
}

public class SummaryStatisticsDto
{
    public int TotalProperties { get; set; }
    public int TotalLocations { get; set; }
    public int TotalSafetyBoxes { get; set; }
    public int TotalItems { get; set; }
    public int TotalSuppliers { get; set; }
    public int TotalEmployees { get; set; }
    public int TotalUsers { get; set; }
    public decimal TotalAssetValue { get; set; }
}

public class RequisitionStatisticsDto
{
    public int Pending { get; set; }
    public int Approved { get; set; }
    public int Issued { get; set; }
    public int Completed { get; set; }
    public int Rejected { get; set; }
    public int Total { get; set; }
    public decimal CompletionRate { get; set; }
    public decimal AverageProcessingTime { get; set; }
}

public class ReceivingStatisticsDto
{
    public int PendingInspection { get; set; }
    public int Approved { get; set; }
    public int Rejected { get; set; }
    public int Total { get; set; }
    public int ThisMonth { get; set; }
    public decimal InspectionPassRate { get; set; }
}

public class StockStatisticsDto
{
    public int TotalStockValue { get; set; }
    public int LowStockItemsCount { get; set; }
    public int OutOfStockItemsCount { get; set; }
    public int InStockItemsCount { get; set; }
    public List<TopStockItemDto> TopItemsByValue { get; set; } = new();
    public List<TopStockItemDto> TopItemsByQuantity { get; set; } = new();
}

public class PropertyStatisticsDto
{
    public decimal TotalValue { get; set; }
    public int ByLocationCount { get; set; }
    public int ByTypeCount { get; set; }
    public int ByCategoryCount { get; set; }
    public decimal AverageValue { get; set; }
    public decimal MaxValue { get; set; }
    public decimal MinValue { get; set; }
    public List<PropertyByLocationDto> PropertiesByLocation { get; set; } = new();
    public List<PropertyByTypeDto> PropertiesByType { get; set; } = new();
    public List<PropertyByCategoryDto> PropertiesByCategory { get; set; } = new();
    public List<PropertyAcquisitionDto> RecentAcquisitions { get; set; } = new();
}

public class ChartDataDto
{
    public List<ChartSeriesDto> RequisitionsByStatus { get; set; } = new();
    public List<ChartSeriesDto> PropertiesByLocation { get; set; } = new();
    public List<ChartSeriesDto> PropertiesByType { get; set; } = new();
    public List<ChartSeriesDto> StockMovementsByMonth { get; set; } = new();
    public List<ChartSeriesDto> ReceivingByStatus { get; set; } = new();
    public List<ChartSeriesDto> MonthlyRequisitions { get; set; } = new();
    public List<ChartSeriesDto> TopItemsByIssuance { get; set; } = new();
}

public class ChartSeriesDto
{
    public string Label { get; set; } = string.Empty;
    public int Value { get; set; }
    public string? Color { get; set; }
    public string? BackgroundColor { get; set; }
    public string? BorderColor { get; set; }
}

public class RecentActivityDto
{
    public Guid Id { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string EntityTitle { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string UserAvatar { get; set; } = string.Empty;
    public DateTime ActionDate { get; set; }
    public string TimeAgo { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class AlertsDto
{
    public List<LowStockAlertDto> LowStockAlerts { get; set; } = new();
    public List<PendingTaskDto> PendingTasks { get; set; } = new();
    public List<ExpiringWarrantyDto> ExpiringWarranties { get; set; } = new();
    public List<OverdueTaskDto> OverdueTasks { get; set; } = new();
}

public class LowStockAlertDto
{
    public Guid ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public int CurrentStock { get; set; }
    public int MinStockLevel { get; set; }
    public int Deficit { get; set; }
    public string Location { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty; // "Critical", "Warning", "Info"
    public string UnitOfMeasure { get; set; } = string.Empty;
    public DateTime LastRestocked { get; set; }
}

public class PendingTaskDto
{
    public Guid Id { get; set; }
    public string TaskType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime DueDate { get; set; }
    public string Priority { get; set; } = string.Empty; // "High", "Medium", "Low"
    public string AssignedTo { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
}

public class ExpiringWarrantyDto
{
    public Guid PropertyId { get; set; }
    public string PropertyName { get; set; } = string.Empty;
    public string TagNumber { get; set; } = string.Empty;
    public DateTime PurchaseDate { get; set; }
    public DateTime WarrantyExpiryDate { get; set; }
    public int DaysRemaining { get; set; }
    public string Severity { get; set; } = string.Empty;
}

public class OverdueTaskDto
{
    public Guid Id { get; set; }
    public string TaskType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public int DaysOverdue { get; set; }
    public string AssignedTo { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class QuickActionDto
{
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string Permission { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
}

public class TopStockItemDto
{
    public Guid ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Value { get; set; }
}

public class PropertyByLocationDto
{
    public string LocationName { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal TotalValue { get; set; }
    public string Color { get; set; } = string.Empty;
}

public class PropertyByTypeDto
{
    public string TypeName { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal TotalValue { get; set; }
    public string Color { get; set; } = string.Empty;
}

public class PropertyByCategoryDto
{
    public string CategoryName { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal TotalValue { get; set; }
    public string Color { get; set; } = string.Empty;
}

public class PropertyAcquisitionDto
{
    public Guid Id { get; set; }
    public string TagNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime PurchaseDate { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalValue { get; set; }
    public string Location { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
}