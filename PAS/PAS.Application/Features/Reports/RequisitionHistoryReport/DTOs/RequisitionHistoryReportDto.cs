namespace Application.Features.Reports.RequisitionHistoryReport.Dtos;

public class RequisitionHistoryReportDto
{
    public DateTime GeneratedAt { get; set; }
    public string GeneratedBy { get; set; } = string.Empty;
    public ReportPeriod Period { get; set; } = new();
    public RequisitionSummary Summary { get; set; } = new();
    public List<RequisitionByStatusDto> ByStatus { get; set; } = new();
    public List<RequisitionByDepartmentDto> ByDepartment { get; set; } = new();
    public List<RequisitionByMonthDto> ByMonth { get; set; } = new();
    public List<RequisitionDetailDto> Requisitions { get; set; } = new();
}

public class ReportPeriod
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
}

public class RequisitionSummary
{
    public int TotalRequisitions { get; set; }
    public int TotalItems { get; set; }
    public long TotalQuantity { get; set; }
    public long IssuedQuantity { get; set; }
    public decimal TotalValue { get; set; }
    public int PendingCount { get; set; }
    public int ApprovedCount { get; set; }
    public int RejectedCount { get; set; }
    public int IssuedCount { get; set; }
    public int CompletedCount { get; set; }
    public double FulfillmentRate => TotalQuantity > 0 ?
        (double)IssuedQuantity / TotalQuantity * 100 : 0;
}

public class RequisitionByStatusDto
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
    public int Items { get; set; }
    public long Quantity { get; set; }
    public double Percentage { get; set; }
}

public class RequisitionByDepartmentDto
{
    public string Department { get; set; } = string.Empty;
    public int Count { get; set; }
    public int Requestors { get; set; }
    public long Quantity { get; set; }
    public double Percentage { get; set; }
}

public class RequisitionByMonthDto
{
    public string Month { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Count { get; set; }
    public long Quantity { get; set; }
    public long IssuedQuantity { get; set; }
}

public class RequisitionDetailDto
{
    public Guid Id { get; set; }
    public string SRNumber { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; }
    public string RequesterName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int ItemCount { get; set; }
    public int TotalQuantity { get; set; }
    public int IssuedQuantity { get; set; }
    public string? ApproverName { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public string? SIVNumber { get; set; }
    public DateTime? IssueDate { get; set; }
    public List<RequisitionItemDto> Items { get; set; } = new();
}

public class RequisitionItemDto
{
    public string ItemName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public int RequestedQty { get; set; }
    public int IssuedQty { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
}