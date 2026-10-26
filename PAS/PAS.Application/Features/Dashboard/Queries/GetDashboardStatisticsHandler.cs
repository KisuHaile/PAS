using Application.Features.Dashboard.Dtos;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Dashboard.Queries;

public class GetDashboardStatisticsHandler : IRequestHandler<GetDashboardStatisticsQuery, Result<DashboardDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<GetDashboardStatisticsHandler> _logger;

    public GetDashboardStatisticsHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        ILogger<GetDashboardStatisticsHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Result<DashboardDto>> Handle(GetDashboardStatisticsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var dashboard = new DashboardDto();

            // Get summary statistics in parallel for better performance
            await Task.WhenAll(
                GetSummaryStatistics(dashboard, cancellationToken),
                GetRequisitionStatistics(dashboard, cancellationToken),
                GetReceivingStatistics(dashboard, cancellationToken),
                GetStockStatistics(dashboard, cancellationToken),
                GetPropertyStatistics(dashboard, cancellationToken),
                GetRecentActivities(dashboard, cancellationToken),
                GetLowStockAlerts(dashboard, cancellationToken),
                GetPendingTasks(dashboard, cancellationToken),
                GetChartData(dashboard, cancellationToken)
            );

            // Add quick actions based on user permissions
            AddQuickActions(dashboard);

            return Result<DashboardDto>.Success(dashboard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating dashboard statistics");
            return Result<DashboardDto>.Failure("An error occurred while generating dashboard statistics.");
        }
    }

    private async Task GetSummaryStatistics(DashboardDto dashboard, CancellationToken cancellationToken)
    {
        dashboard.TotalProperties = await _context.Properties
            .CountAsync(p => !p.IsDeleted, cancellationToken);

        dashboard.TotalLocations = await _context.PropertyLocations
            .CountAsync(l => !l.IsDeleted, cancellationToken);

        dashboard.TotalSafetyBoxes = await _context.SafetyBoxes
            .CountAsync(s => !s.IsDeleted, cancellationToken);

        dashboard.TotalItems = await _context.ItemMasters
            .CountAsync(i => !i.IsDeleted, cancellationToken);

        dashboard.TotalSuppliers = await _context.Suppliers
            .CountAsync(s => !s.IsDeleted, cancellationToken);

        dashboard.TotalEmployees = await _context.Employees
            .CountAsync(e => !e.IsDeleted, cancellationToken);
    }

    private async Task GetRequisitionStatistics(DashboardDto dashboard, CancellationToken cancellationToken)
    {
        dashboard.PendingRequisitions = await _context.ServiceRequests
            .CountAsync(sr => !sr.IsDeleted && sr.Status == "Pending", cancellationToken);

        dashboard.ApprovedRequisitions = await _context.ServiceRequests
            .CountAsync(sr => !sr.IsDeleted && sr.Status == "Approved", cancellationToken);

        dashboard.IssuedRequisitions = await _context.ServiceRequests
            .CountAsync(sr => !sr.IsDeleted && sr.Status == "Issued", cancellationToken);

        dashboard.CompletedRequisitions = await _context.ServiceRequests
            .CountAsync(sr => !sr.IsDeleted && sr.Status == "Completed", cancellationToken);

        dashboard.RejectedRequisitions = await _context.ServiceRequests
            .CountAsync(sr => !sr.IsDeleted && sr.Status == "Rejected", cancellationToken);

        // Chart data for requisitions by status
        dashboard.RequisitionsByStatus = new List<ChartDataDto>
        {
            new() { Label = "Pending", Value = dashboard.PendingRequisitions, Color = "#FFC107" },
            new() { Label = "Approved", Value = dashboard.ApprovedRequisitions, Color = "#17A2B8" },
            new() { Label = "Issued", Value = dashboard.IssuedRequisitions, Color = "#007BFF" },
            new() { Label = "Completed", Value = dashboard.CompletedRequisitions, Color = "#28A745" },
            new() { Label = "Rejected", Value = dashboard.RejectedRequisitions, Color = "#DC3545" }
        };
    }

    private async Task GetReceivingStatistics(DashboardDto dashboard, CancellationToken cancellationToken)
    {
        dashboard.PendingInspections = await _context.ReceivingNotes
            .CountAsync(r => !r.IsDeleted && r.Status == "PendingInspection", cancellationToken);

        dashboard.ApprovedReceiving = await _context.ReceivingNotes
            .CountAsync(r => !r.IsDeleted && r.Status == "Approved", cancellationToken);

        dashboard.RejectedReceiving = await _context.ReceivingNotes
            .CountAsync(r => !r.IsDeleted && r.Status == "Rejected", cancellationToken);

        // Chart data for receiving by status
        dashboard.ReceivingByStatus = new List<ChartDataDto>
        {
            new() { Label = "Pending Inspection", Value = dashboard.PendingInspections, Color = "#FFC107" },
            new() { Label = "Approved", Value = dashboard.ApprovedReceiving, Color = "#28A745" },
            new() { Label = "Rejected", Value = dashboard.RejectedReceiving, Color = "#DC3545" }
        };
    }

    private async Task GetStockStatistics(DashboardDto dashboard, CancellationToken cancellationToken)
    {
        var inventoryStocks = await _context.InventoryStocks
            .Include(i => i.Item)
            .Where(i => !i.IsDeleted)
            .ToListAsync(cancellationToken);

        dashboard.TotalStockValue = inventoryStocks.Sum(i => i.CurrentQuantity * (i.Item?.UnitPrice ?? 0));

        dashboard.LowStockItemsCount = inventoryStocks
            .Count(i => i.Item != null && i.CurrentQuantity <= i.Item.MinStockLevel && i.CurrentQuantity > 0);

        dashboard.OutOfStockItemsCount = inventoryStocks
            .Count(i => i.CurrentQuantity == 0);
    }

    private async Task GetPropertyStatistics(DashboardDto dashboard, CancellationToken cancellationToken)
    {
        var properties = await _context.Properties
            .Include(p => p.PropertyType)
            .Include(p => p.Location)
            .Where(p => !p.IsDeleted)
            .ToListAsync(cancellationToken);

        dashboard.TotalPropertyValue = properties.Sum(p => p.UnitPrice * p.Quantity);
        dashboard.PropertiesByLocation = properties.GroupBy(p => p.LocationId).Count();
        dashboard.PropertiesByType = properties.GroupBy(p => p.PropertyTypeId).Count();

        // Chart data for properties by location
        var propertiesByLocation = properties
            .GroupBy(p => p.Location?.Name ?? "Unknown")
            .Select(g => new ChartDataDto
            {
                Label = g.Key,
                Value = g.Count()
            })
            .OrderByDescending(c => c.Value)
            .Take(5)
            .ToList();

        dashboard.PropertiesByLocationChart = propertiesByLocation;

        // Stock movements by month (last 6 months)
        var sixMonthsAgo = DateTime.UtcNow.AddMonths(-6);
        var stockMovements = await _context.StockLedgers
            .Where(l => l.CreatedDate >= sixMonthsAgo)
            .ToListAsync(cancellationToken);

        dashboard.StockMovementsByMonth = stockMovements
            .GroupBy(l => new { l.CreatedDate.Year, l.CreatedDate.Month })
            .Select(g => new ChartDataDto
            {
                Label = $"{g.Key.Year}-{g.Key.Month:D2}",
                Value = g.Sum(l => Math.Abs(l.QuantityChange))
            })
            .OrderBy(c => c.Label)
            .ToList();
    }

    private async Task GetRecentActivities(DashboardDto dashboard, CancellationToken cancellationToken)
    {
        var recentActivities = await _context.AuditTrails
            .OrderByDescending(a => a.ActionDate)
            .Take(10)
            .ToListAsync(cancellationToken);

        dashboard.RecentActivities = recentActivities.Select(a => new RecentActivityDto
        {
            Id = a.Id,
            Action = a.Action,
            EntityName = a.EntityName,
            EntityId = a.EntityId.ToString(),
            UserName = "System",
            ActionDate = a.ActionDate,
            TimeAgo = GetTimeAgo(a.ActionDate),
            Icon = GetActivityIcon(a.Action),
            Color = GetActivityColor(a.Action)
        }).ToList();
    }

    private async Task GetLowStockAlerts(DashboardDto dashboard, CancellationToken cancellationToken)
    {
        var lowStockItems = await _context.InventoryStocks
            .Include(i => i.Item)
            .Include(i => i.Shelf)
            .Where(i => i.Item != null && i.CurrentQuantity <= i.Item.MinStockLevel && !i.IsDeleted)
            .OrderBy(i => i.CurrentQuantity)
            .Take(10)
            .ToListAsync(cancellationToken);

        dashboard.LowStockAlerts = lowStockItems.Select(i => new LowStockAlertDto
        {
            ItemId = i.ItemId,
            ItemName = i.Item?.ItemName ?? "Unknown",
            SKU = i.Item?.SKU ?? string.Empty,
            CurrentStock = i.CurrentQuantity,
            MinStockLevel = i.Item?.MinStockLevel ?? 0,
            Deficit = (i.Item?.MinStockLevel ?? 0) - i.CurrentQuantity,
            Location = i.Shelf != null ? $"{i.Shelf.Aisle}-{i.Shelf.Rack}-{i.Shelf.ShelfNumber}" : "Unknown",
            Severity = GetStockSeverity(i.CurrentQuantity, i.Item?.MinStockLevel ?? 0)
        }).ToList();

        dashboard.LowStockItemsCount = lowStockItems.Count;
    }

    private async Task GetPendingTasks(DashboardDto dashboard, CancellationToken cancellationToken)
    {
        var pendingTasks = new List<PendingTaskDto>();

        // Pending requisitions for approval
        if (_currentUser.HasPermission(Permissions.Requisitions.Approve))
        {
            var pendingRequisitions = await _context.ServiceRequests
                .Where(s => s.Status == "Pending" && !s.IsDeleted)
                .Take(5)
                .ToListAsync(cancellationToken);

            pendingTasks.AddRange(pendingRequisitions.Select(r => new PendingTaskDto
            {
                Id = r.Id,
                TaskType = "Requisition Approval",
                Description = $"Requisition {r.SRNumber}",
                Reference = r.SRNumber,
                DueDate = r.RequestDate.AddDays(2),
                Priority = "High",
                AssignedTo = string.Empty,
                Status = r.Status
            }));
        }

        // Pending inspections
        if (_currentUser.HasPermission(Permissions.Receiving.Inspect))
        {
            var pendingInspections = await _context.ReceivingNotes
                .Include(r => r.Supplier)
                .Where(r => r.Status == "PendingInspection" && !r.IsDeleted)
                .Take(5)
                .ToListAsync(cancellationToken);

            pendingTasks.AddRange(pendingInspections.Select(r => new PendingTaskDto
            {
                Id = r.Id,
                TaskType = "Inspection",
                Description = $"Inspect GRN {r.GRNNumber}",
                Reference = r.GRNNumber,
                DueDate = r.ReceivedDate.AddDays(1),
                Priority = "Medium",
                AssignedTo = r.Supplier?.SupplierName ?? "Unknown",
                Status = r.Status
            }));
        }

        dashboard.PendingTasks = pendingTasks;
    }

    private void AddQuickActions(DashboardDto dashboard)
    {
        var quickActions = new List<QuickActionDto>();

        if (_currentUser.HasPermission(Permissions.Properties.Create))
        {
            quickActions.Add(new QuickActionDto
            {
                Name = "Add Property",
                Icon = "bi-plus-circle",
                Route = "/properties/create",
                Color = "primary",
                Permission = Permissions.Properties.Create
            });
        }

        if (_currentUser.HasPermission(Permissions.Requisitions.Create))
        {
            quickActions.Add(new QuickActionDto
            {
                Name = "New Requisition",
                Icon = "bi-cart-plus",
                Route = "/requisitions/create",
                Color = "success",
                Permission = Permissions.Requisitions.Create
            });
        }

        if (_currentUser.HasPermission(Permissions.Receiving.Create))
        {
            quickActions.Add(new QuickActionDto
            {
                Name = "Create Receiving Note",
                Icon = "bi-box-seam",
                Route = "/receiving/create",
                Color = "info",
                Permission = Permissions.Receiving.Create
            });
        }

        if (_currentUser.HasPermission(Permissions.Reports.View))
        {
            quickActions.Add(new QuickActionDto
            {
                Name = "Generate Report",
                Icon = "bi-file-earmark-bar-graph",
                Route = "/reports",
                Color = "warning",
                Permission = Permissions.Reports.View
            });
        }

        dashboard.QuickActions = quickActions;
    }

    private async Task GetChartData(DashboardDto dashboard, CancellationToken cancellationToken)
    {
        var sevenDaysAgo = DateTime.UtcNow.AddDays(-6).Date;

        var createdProperties = await _context.Properties
            .Where(p => p.CreatedAt >= sevenDaysAgo && !p.IsDeleted)
            .GroupBy(p => p.CreatedAt.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        dashboard.DailyCreatedProperties = Enumerable.Range(0, 7)
            .Select(i => sevenDaysAgo.AddDays(i))
            .Select(date => new ChartDataDto
            {
                Label = date.ToString("MM-dd"),
                Value = createdProperties.FirstOrDefault(x => x.Date == date)?.Count ?? 0
            })
            .ToList();
    }

    private string GetTimeAgo(DateTime dateTime)
    {
        var timeSpan = DateTime.UtcNow - dateTime;

        if (timeSpan.TotalSeconds < 60)
            return "just now";
        if (timeSpan.TotalMinutes < 60)
            return $"{(int)timeSpan.TotalMinutes} minute{(timeSpan.TotalMinutes >= 2 ? "s" : "")} ago";
        if (timeSpan.TotalHours < 24)
            return $"{(int)timeSpan.TotalHours} hour{(timeSpan.TotalHours >= 2 ? "s" : "")} ago";
        if (timeSpan.TotalDays < 30)
            return $"{(int)timeSpan.TotalDays} day{(timeSpan.TotalDays >= 2 ? "s" : "")} ago";
        if (timeSpan.TotalDays < 365)
            return $"{(int)(timeSpan.TotalDays / 30)} month{(timeSpan.TotalDays / 30 >= 2 ? "s" : "")} ago";

        return $"{(int)(timeSpan.TotalDays / 365)} year{(timeSpan.TotalDays / 365 >= 2 ? "s" : "")} ago";
    }

    private string GetActivityIcon(string action)
    {
        return action.ToUpper() switch
        {
            "CREATE" => "bi-plus-circle",
            "UPDATE" => "bi-pencil",
            "DELETE" => "bi-trash",
            "APPROVE" => "bi-check-circle",
            "REJECT" => "bi-x-circle",
            "TRANSFER" => "bi-arrow-left-right",
            "ISSUE" => "bi-box-arrow-right",
            "RECEIVE" => "bi-box-arrow-in-down",
            _ => "bi-clock-history"
        };
    }

    private string GetActivityColor(string action)
    {
        return action.ToUpper() switch
        {
            "CREATE" => "success",
            "UPDATE" => "info",
            "DELETE" => "danger",
            "APPROVE" => "success",
            "REJECT" => "danger",
            "TRANSFER" => "warning",
            "ISSUE" => "primary",
            "RECEIVE" => "secondary",
            _ => "secondary"
        };
    }

    private string GetStockSeverity(int currentStock, int minLevel)
    {
        if (currentStock == 0)
            return "Critical";
        if (currentStock <= minLevel * 0.3)
            return "Critical";
        if (currentStock <= minLevel * 0.6)
            return "Warning";
        return "Info";
    }
}