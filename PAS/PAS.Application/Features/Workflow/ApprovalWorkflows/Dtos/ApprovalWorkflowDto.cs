using Application.Common.Mappings;
using AutoMapper;

namespace Application.Features.Workflow.ApprovalWorkflows.Dtos;

public class ApprovalWorkflowDto : IMapFrom<ApprovalWorkflow>
{
    public Guid Id { get; set; }
    public string WorkflowName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ApproversCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<ApprovalWorkflow, ApprovalWorkflowDto>()
            .ForMember(d => d.ApproversCount, opt => opt.Ignore());
    }
}

public class ApprovalWorkflowDetailDto : ApprovalWorkflowDto
{
    public List<WorkflowApproverDto> Approvers { get; set; } = new();
}

public class WorkflowApproverDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int ApprovalLevel { get; set; }
    public DateTime AssignedAt { get; set; }
}