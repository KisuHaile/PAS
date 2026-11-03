namespace Application.Features.Reports.StockMovementReport.Dtos;

public class StockMovementReportDto
{
    public DateTime GeneratedAt { get; set; }
    public string GeneratedBy { get; set; } = string.Empty;
    public ReportPeriod Period { get; set; } = new();
    public MovementSummary Summary { get; set; } = new();
    public List<MovementByTypeDto> ByType { get; set; } = new();
    public List<MovementByItemDto> TopMovingItems { get; set; } = new();
    public List<MovementTrendDto> DailyTrend { get; set; } = new();
    public List<StockMovementDetailDto> Movements { get; set; } = new();
}

public class ReportPeriod
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
}

public class MovementSummary
{
    public int TotalTransactions { get; set; }
    public int TotalInbound { get; set; }
    public int TotalOutbound { get; set; }
    public int TotalAdjustments { get; set; }
    public long TotalQuantityIn { get; set; }
    public long TotalQuantityOut { get; set; }
    public long NetMovement => TotalQuantityIn - TotalQuantityOut;
    public int UniqueItemsMoved { get; set; }
}

public class MovementByTypeDto
{
    public string TransactionType { get; set; } = string.Empty;
    public int Count { get; set; }
    public long Quantity { get; set; }
    public double Percentage { get; set; }
}

public class MovementByItemDto
{
    public Guid ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public long TotalQuantity { get; set; }
    public int TransactionCount { get; set; }
    public long InboundQuantity { get; set; }
    public long OutboundQuantity { get; set; }
    public long NetMovement { get; set; }
}

public class MovementTrendDto
{
    public DateTime Date { get; set; }
    public long Inbound { get; set; }
    public long Outbound { get; set; }
    public long Net { get; set; }
}

public class StockMovementDetailDto
{
    public DateTime Date { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public int QuantityChange { get; set; }
    public string Warehouse { get; set; } = string.Empty;
    public string ShelfLocation { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public string PerformedBy { get; set; } = string.Empty;
}