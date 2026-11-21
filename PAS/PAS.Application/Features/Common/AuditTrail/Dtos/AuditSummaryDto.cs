namespace Application.Features.Common.AuditTrail.Dtos;

public class MostActiveUserDto
{
    public Guid? UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int ActionCount { get; set; }
}

public class AuditSummaryDto
{
    public int TotalEvents { get; set; }
    public int TodayEvents { get; set; }
    public int ThisWeekEvents { get; set; }
    public MostActiveUserDto? MostActiveUser { get; set; }
    public int? SuspiciousCount { get; set; }
    public string SuspiciousActivityNote { get; set; } = "Suspicious activity detection is not implemented yet.";
}
