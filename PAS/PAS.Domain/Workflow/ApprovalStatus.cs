using Domain.Common;

namespace Domain.Workflow
{
    public class ApprovalStatus : BaseEntity
    {
        public string StatusName { get; private set; }

        private ApprovalStatus()
        {
        }

        public ApprovalStatus(string name)
        {
            StatusName = name;
        }
    }
}
