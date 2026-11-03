using Domain.Requisition;
using System;

namespace Persistence.Specifications
{
    public class ServiceRequestSpecification : BaseSpecification<ServiceRequest>
    {
        public ServiceRequestSpecification()
        {
            AddInclude(r => r.Details);
        }

        public ServiceRequestSpecification(Guid requesterId)
            : base(r => r.RequesterId == requesterId)
        {
            AddInclude(r => r.Details);
            ApplyOrderByDescending(r => r.RequestDate);
        }
    }
}
