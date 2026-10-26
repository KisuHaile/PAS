using Application.Common.Mappings;
using AutoMapper;

namespace Application.Features.Workflow.Approvers.Dtos;

public class ApproverDto : IMapFrom<Approver>
{
    public Guid Id { get; set; }
    public Guid WorkflowId { get; set; }
    public string WorkflowName { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int ApprovalLevel { get; set; }
    public DateTime CreatedAt { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Approver, ApproverDto>()
            .ForMember(d => d.WorkflowName, opt => opt.MapFrom(s => s.Workflow != null ? s.Workflow.WorkflowName : string.Empty))
            .ForMember(d => d.UserName, opt => opt.Ignore());
    }
}