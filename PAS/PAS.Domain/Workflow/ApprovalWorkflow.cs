using System.Collections.Generic;
using Domain.Common;

namespace Domain.Workflow
{
    public class ApprovalWorkflow : BaseEntity
    {
        public string WorkflowName { get; private set; }

        public string Description { get; private set; }

        public ICollection<Approver> Approvers { get; private set; } = new List<Approver>();

        private ApprovalWorkflow()
        {
        }

        public ApprovalWorkflow(string name, string description)
        {
            WorkflowName = name;
            Description = description;
        }
    }
}
