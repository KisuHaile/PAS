using System;
using Domain.Common;
namespace Domain.Workflow
{
    public class Approver : BaseEntity
    {
        public Guid WorkflowId { get; private set; }

        public Guid UserId { get; private set; }

        public int ApprovalLevel { get; private set; }

        public ApprovalWorkflow Workflow { get; private set; }

        private Approver()
        {
        }

        public Approver(Guid workflowId, Guid userId, int level)
        {
            WorkflowId = workflowId;
            UserId = userId;
            ApprovalLevel = level;
        }
    }
}

