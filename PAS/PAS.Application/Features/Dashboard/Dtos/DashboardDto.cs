namespace Application.Features.Dashboard.Dtos;

public class DashboardDto
{
    // Summary Statistics
    public int TotalProperties { get; set; }
    public int TotalLocations { get; set; }
    public int TotalSafetyBoxes { get; set; }
    public int TotalItems { get; set; }
    public int TotalSuppliers { get; set; }
    public int TotalEmployees { get; set; }

    // Requisition Statistics
    public int PendingRequisitions { get; set; }
    public int ApprovedRequisitions { get; set; }
    public int IssuedRequisitions { get; set; }
    public int CompletedRequisitions { get; set; }
    public int RejectedRequisitions { get; set; }

    // Receiving Statistics
    public int PendingInspections { get; set; }
    public int ApprovedReceiving { get; set; }
    public int RejectedReceiving { get; set; }

    // Stock Statistics
    public decimal TotalStockValue { get; set; }
    public int LowStockItemsCount { get; set; }
    public int OutOfStockItemsCount { get; set; }

    // Property Statistics
    public decimal TotalPropertyValue { get; set; }
    public int PropertiesByLocation { get; set; }
    public int PropertiesByType { get; set; }

    // Charts Data
    public List<ChartDataDto> RequisitionsByStatus { get; set; } = new();
    public List<ChartDataDto> PropertiesByLocationChart { get; set; } = new();
    public List<ChartDataDto> StockMovementsByMonth { get; set; } = new();
    public List<ChartDataDto> ReceivingByStatus { get; set; } = new();
    public List<ChartDataDto> DailyCreatedProperties { get; set; } = new();

    // Recent Activities
    public List<RecentActivityDto> RecentActivities { get; set; } = new();

    // Alerts
    public List<LowStockAlertDto> LowStockAlerts { get; set; } = new();
    public List<PendingTaskDto> PendingTasks { get; set; } = new();

    // Quick Actions
    public List<QuickActionDto> QuickActions { get; set; } = new();
}

public class ChartDataDto
{
    public string Label { get; set; } = string.Empty;
    public int Value { get; set; }
    public string? Color { get; set; }
}

public class RecentActivityDto
{
    public Guid Id { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateTime ActionDate { get; set; }
    public string TimeAgo { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
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
}

public class PendingTaskDto
{
    public Guid Id { get; set; }
    public string TaskType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public string Priority { get; set; } = string.Empty; // "High", "Medium", "Low"
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
}