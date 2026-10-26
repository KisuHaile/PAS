using System;
namespace Domain.Common
{
    public class AuditTrail : BaseEntity
    {
        public Guid UserId { get; private set; }

        public string Action { get; private set; }

        public string EntityName { get; private set; }

        public Guid EntityId { get; private set; }

        public DateTime ActionDate { get; private set; }

        public AuditTrail(Guid userId, string action, string entityName, Guid entityId)
        {
            UserId = userId;
            Action = action;
            EntityName = entityName;
            EntityId = entityId;
            ActionDate = DateTime.UtcNow;
        }
    }
}