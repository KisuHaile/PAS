using Application.Common.Mappings;
using AutoMapper;

namespace Application.Features.Workflow.ApprovalStatuses.Dtos;

public class ApprovalStatusDto : IMapFrom<ApprovalStatus>
{
    public Guid Id { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<ApprovalStatus, ApprovalStatusDto>();
    }
}