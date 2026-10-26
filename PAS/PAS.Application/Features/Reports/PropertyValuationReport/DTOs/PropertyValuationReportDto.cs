namespace Application.Features.Reports.PropertyValuationReport.Dtos;

public class PropertyValuationReportDto
{
    public DateTime GeneratedAt { get; set; }
    public string GeneratedBy { get; set; } = string.Empty;
    public ReportFilterInfo Filters { get; set; } = new();
    public ValuationSummary Summary { get; set; } = new();
    public List<ValuationByTypeDto> ByType { get; set; } = new();
    public List<ValuationByLocationDto> ByLocation { get; set; } = new();
    public List<ValuationByCategoryDto> ByCategory { get; set; } = new();
    public List<ValuationByAgeDto> ByAge { get; set; } = new();
    public List<PropertyValuationDetailDto> Properties { get; set; } = new();
}

public class ReportFilterInfo
{
    public DateTime? AsOfDate { get; set; }
    public Guid? LocationId { get; set; }
    public Guid? PropertyTypeId { get; set; }
    public Guid? PropertyCategoryId { get; set; }
    public decimal? MinValue { get; set; }
    public decimal? MaxValue { get; set; }
    public string? SearchTerm { get; set; }
}

public class ValuationSummary
{
    public int TotalProperties { get; set; }
    public int TotalItems { get; set; }
    public decimal TotalValue { get; set; }
    public decimal AverageValue { get; set; }
    public decimal MinValue { get; set; }
    public decimal MaxValue { get; set; }
    public int PropertiesWithoutLocation { get; set; }
    public int PropertiesWithoutSafetyBox { get; set; }
}

public class ValuationByTypeDto
{
    public string PropertyType { get; set; } = string.Empty;
    public int Count { get; set; }
    public int Items { get; set; }
    public decimal TotalValue { get; set; }
    public double Percentage { get; set; }
}

public class ValuationByLocationDto
{
    public string Location { get; set; } = string.Empty;
    public int Count { get; set; }
    public int Items { get; set; }
    public decimal TotalValue { get; set; }
    public double Percentage { get; set; }
}

public class ValuationByCategoryDto
{
    public string Category { get; set; } = string.Empty;
    public int Count { get; set; }
    public int Items { get; set; }
    public decimal TotalValue { get; set; }
    public double Percentage { get; set; }
}

public class ValuationByAgeDto
{
    public string AgeRange { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal TotalValue { get; set; }
    public int MinYears { get; set; }
    public int MaxYears { get; set; }
}

public class PropertyValuationDetailDto
{
    public Guid Id { get; set; }
    public string TagNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public string PropertyType { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalValue { get; set; }
    public DateTime PurchaseDate { get; set; }
    public int AgeInYears => (DateTime.Now - PurchaseDate).Days / 365;
    public string Location { get; set; } = string.Empty;
    public string SafetyBox { get; set; } = string.Empty;
    public int? ShelfNumber { get; set; }
}

public class PropertyIssuanceReportDto
{
    public DateTime GeneratedAt { get; set; }
    public string GeneratedBy { get; set; } = string.Empty;
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int TotalVouchers { get; set; }
    public int TotalIssuedItems { get; set; }
    public List<PropertyIssuanceItemDto> Items { get; set; } = new();
}

public class PropertyIssuanceItemDto
{
    public Guid VoucherId { get; set; }
    public string SIVNumber { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public string IssuedBy { get; set; } = string.Empty;
    public string RecipientName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}