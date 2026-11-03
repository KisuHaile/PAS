using System;

namespace Application.Features.Requisition.ServiceRequests.Dtos
{
    public class ServiceRequestDto
    {
        public Guid Id { get; set; }
        public string SRNumber { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
