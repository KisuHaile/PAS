namespace Application.Features.Reports.DisposalReport.Dtos;

public class DisposalReportDto
{
    public DateTime GeneratedAt { get; set; }
    public string GeneratedBy { get; set; } = string.Empty;
    public ReportPeriod Period { get; set; } = new();
    public DisposalSummary Summary { get; set; } = new();
    public List<DisposalByReasonDto> ByReason { get; set; } = new();
    public List<DisposalByMethodDto> ByMethod { get; set; } = new();
    public List<DisposalByMonthDto> ByMonth { get; set; } = new();
    public List<DisposalDetailDto> Disposals { get; set; } = new();
}

public class ReportPeriod
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
}

public class DisposalSummary
{
    public int TotalDisposals { get; set; }
    public int TotalItems { get; set; }
    public long TotalQuantity { get; set; }
    public decimal TotalEstimatedValue { get; set; }
    public decimal AverageValuePerItem { get; set; }
    public int PendingApprovals { get; set; }
    public int ApprovedDisposals { get; set; }
    public int RejectedDisposals { get; set; }
}

public class DisposalByReasonDto
{
    public string Reason { get; set; } = string.Empty;
    public int Count { get; set; }
    public long Quantity { get; set; }
    public decimal TotalValue { get; set; }
    public double Percentage { get; set; }
}

public class DisposalByMethodDto
{
    public string Method { get; set; } = string.Empty;
    public int Count { get; set; }
    public long Quantity { get; set; }
    public decimal TotalValue { get; set; }
    public double Percentage { get; set; }
}

public class DisposalByMonthDto
{
    public string Month { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Count { get; set; }
    public long Quantity { get; set; }
    public decimal Value { get; set; }
}

public class DisposalDetailDto
{
    public Guid Id { get; set; }
    public DateTime DisposalDate { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string ItemSKU { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal EstimatedValue { get; set; }
    public string DisposedBy { get; set; } = string.Empty;
    public string ApprovedBy { get; set; } = string.Empty;
    public DateTime? ApprovedDate { get; set; }
}