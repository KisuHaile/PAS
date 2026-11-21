namespace Application.Features.Common.AuditTrail.Dtos;

public class AuditTrailFiltersDto
{
    public List<string> Actions { get; set; } = new();
    public List<string> EntityNames { get; set; } = new();
}
