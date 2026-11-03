using Application.Common.Mappings;
using AutoMapper;

namespace Application.Features.Common.AuditTrail.Dtos;

public class AuditTrailDto : IMapFrom<Domain.Common.AuditTrail>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public DateTime ActionDate { get; set; }
    public string Details { get; set; } = string.Empty;

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Domain.Common.AuditTrail, AuditTrailDto>()
            .ForMember(d => d.UserName, opt => opt.Ignore())
            .ForMember(d => d.Details, opt => opt.MapFrom(s => $"{s.Action} on {s.EntityName} with ID: {s.EntityId}"));
    }
}

public class AuditTrailListDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public DateTime ActionDate { get; set; }
}

public class AuditTrailDetailDto : AuditTrailDto
{
    public Dictionary<string, object>? Changes { get; set; }
}