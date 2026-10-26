using System;
using System.Collections.Generic;
using Domain.Common;
using Domain.Workflow;

namespace Domain.Requisition
{
    public class ServiceRequest : BaseEntity
    {
        public string SRNumber { get; private set; }

        public Guid RequesterId { get; private set; }

        public Guid? ApprovalStatusId { get; private set; }

        public DateTime RequestDate { get; private set; }

        public string Status { get; private set; }

        public ApprovalStatus? ApprovalStatus { get; private set; }

        public StoreIssueVoucher? StoreIssueVoucher { get; private set; }

        public ICollection<SR_Detail> Details { get; private set; } = new List<SR_Detail>();

        private ServiceRequest()
        {
        }

        public ServiceRequest(string number, Guid requester)
        {
            SRNumber = number;
            RequesterId = requester;
            RequestDate = DateTime.UtcNow;
            Status = "Pending";
        }

        public void Approve()
        {
            Status = "Approved";
        }
    }
}