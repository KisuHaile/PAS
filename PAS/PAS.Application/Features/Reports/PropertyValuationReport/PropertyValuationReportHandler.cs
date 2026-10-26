using Application.Features.Reports.PropertyValuationReport.Dtos;
using MediatR;

namespace Application.Features.Reports.PropertyValuationReport;

public class PropertyValuationReportQueryHandler : IRequestHandler<PropertyValuationReportQuery, Result<PropertyValuationReportDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeService _dateTime;

    public PropertyValuationReportQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IDateTimeService dateTime)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTime = dateTime;
    }

    public async Task<Result<PropertyValuationReportDto>> Handle(PropertyValuationReportQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Properties
            .Include(p => p.PropertyType)
            .Include(p => p.PropertyCategory)
            .Include(p => p.Location)
            .Include(p => p.SafetyBox)
            .Where(p => !p.IsDeleted)
            .AsNoTracking();

        // Apply filters
        if (request.LocationId.HasValue)
        {
            query = query.Where(p => p.LocationId == request.LocationId);
        }

        if (request.PropertyTypeId.HasValue)
        {
            query = query.Where(p => p.PropertyTypeId == request.PropertyTypeId);
        }

        if (request.PropertyCategoryId.HasValue)
        {
            query = query.Where(p => p.PropertyCategoryId == request.PropertyCategoryId);
        }

        if (request.MinValue.HasValue)
        {
            query = query.Where(p => p.UnitPrice * p.Quantity >= request.MinValue);
        }

        if (request.MaxValue.HasValue)
        {
            query = query.Where(p => p.UnitPrice * p.Quantity <= request.MaxValue);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(p =>
                p.TagNumber.Contains(request.SearchTerm) ||
                p.Name.Contains(request.SearchTerm) ||
                p.SerialNumber.Contains(request.SearchTerm));
        }

        var properties = await query.ToListAsync(cancellationToken);

        // Calculate summary
        var totalValue = properties.Sum(p => p.UnitPrice * p.Quantity);
        var summary = new ValuationSummary
        {
            TotalProperties = properties.Count,
            TotalItems = properties.Sum(p => p.Quantity),
            TotalValue = totalValue,
            AverageValue = properties.Any() ? totalValue / properties.Count : 0,
            MinValue = properties.Any() ? properties.Min(p => p.UnitPrice * p.Quantity) : 0,
            MaxValue = properties.Any() ? properties.Max(p => p.UnitPrice * p.Quantity) : 0,
            PropertiesWithoutLocation = properties.Count(p => p.LocationId == Guid.Empty),
            PropertiesWithoutSafetyBox = properties.Count(p => !p.SafetyBoxId.HasValue)
        };

        // Group by property type
        var byType = properties
            .GroupBy(p => p.PropertyType?.Name ?? "Unknown")
            .Select(g => new ValuationByTypeDto
            {
                PropertyType = g.Key,
                Count = g.Count(),
                Items = g.Sum(p => p.Quantity),
                TotalValue = g.Sum(p => p.UnitPrice * p.Quantity),
                Percentage = totalValue > 0 ?
                    (double)(g.Sum(p => p.UnitPrice * p.Quantity) / totalValue) * 100 : 0
            })
            .OrderByDescending(t => t.TotalValue)
            .ToList();

        // Group by location
        var byLocation = properties
            .GroupBy(p => p.Location?.Name ?? "Unassigned")
            .Select(g => new ValuationByLocationDto
            {
                Location = g.Key,
                Count = g.Count(),
                Items = g.Sum(p => p.Quantity),
                TotalValue = g.Sum(p => p.UnitPrice * p.Quantity),
                Percentage = totalValue > 0 ?
                    (double)(g.Sum(p => p.UnitPrice * p.Quantity) / totalValue) * 100 : 0
            })
            .OrderByDescending(l => l.TotalValue)
            .ToList();

        // Group by category
        var byCategory = properties
            .GroupBy(p => p.PropertyCategory?.Name ?? "Uncategorized")
            .Select(g => new ValuationByCategoryDto
            {
                Category = g.Key,
                Count = g.Count(),
                Items = g.Sum(p => p.Quantity),
                TotalValue = g.Sum(p => p.UnitPrice * p.Quantity),
                Percentage = totalValue > 0 ?
                    (double)(g.Sum(p => p.UnitPrice * p.Quantity) / totalValue) * 100 : 0
            })
            .OrderByDescending(c => c.TotalValue)
            .ToList();

        // Group by age
        var now = _dateTime.Now;
        var byAge = new List<ValuationByAgeDto>
        {
            new() { AgeRange = "Less than 1 year", MinYears = 0, MaxYears = 1 },
            new() { AgeRange = "1-3 years", MinYears = 1, MaxYears = 3 },
            new() { AgeRange = "3-5 years", MinYears = 3, MaxYears = 5 },
            new() { AgeRange = "5-10 years", MinYears = 5, MaxYears = 10 },
            new() { AgeRange = "Over 10 years", MinYears = 10, MaxYears = 100 }
        };

        foreach (var ageGroup in byAge)
        {
            var ageProperties = properties.Where(p =>
                (now - p.PurchaseDate).TotalDays / 365 >= ageGroup.MinYears &&
                (now - p.PurchaseDate).TotalDays / 365 < ageGroup.MaxYears).ToList();

            ageGroup.Count = ageProperties.Count;
            ageGroup.TotalValue = ageProperties.Sum(p => p.UnitPrice * p.Quantity);
        }

        // Detail list
        var details = properties.Select(p => new PropertyValuationDetailDto
        {
            Id = p.Id,
            TagNumber = p.TagNumber,
            Name = p.Name,
            SerialNumber = p.SerialNumber,
            PropertyType = p.PropertyType?.Name ?? "Unknown",
            Category = p.PropertyCategory?.Name ?? "Uncategorized",
            UnitPrice = p.UnitPrice,
            Quantity = p.Quantity,
            TotalValue = p.UnitPrice * p.Quantity,
            PurchaseDate = p.PurchaseDate,
            Location = p.Location?.Name ?? "Unassigned",
            SafetyBox = p.SafetyBox?.BoxNumber ?? "None",
            ShelfNumber = null // Would need to get from assignment
        }).ToList();

        var report = new PropertyValuationReportDto
        {
            GeneratedAt = _dateTime.UtcNow,
            GeneratedBy = _currentUser.UserName ?? "System",
            Filters = new ReportFilterInfo
            {
                AsOfDate = request.AsOfDate,
                LocationId = request.LocationId,
                PropertyTypeId = request.PropertyTypeId,
                PropertyCategoryId = request.PropertyCategoryId,
                MinValue = request.MinValue,
                MaxValue = request.MaxValue,
                SearchTerm = request.SearchTerm
            },
            Summary = summary,
            ByType = byType,
            ByLocation = byLocation,
            ByCategory = byCategory,
            ByAge = byAge,
            Properties = details
        };

        return Result<PropertyValuationReportDto>.Success(report);
    }
}