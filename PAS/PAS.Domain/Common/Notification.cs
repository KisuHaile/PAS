using System;

namespace Domain.Common
{
    public class Notification : BaseEntity
    {
        public Guid UserId { get; private set; }

        public string Message { get; private set; }

        public bool IsRead { get; private set; }

        public DateTime SentDate { get; private set; }

        private Notification()
        {
        }

        public Notification(Guid userId, string message)
        {
            UserId = userId;
            Message = message;
            SentDate = DateTime.UtcNow;
        }

        public void MarkAsRead()
        {
            IsRead = true;
        }
    }
}