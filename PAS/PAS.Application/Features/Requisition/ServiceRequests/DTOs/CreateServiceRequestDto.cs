namespace Application.Features.Requisition.ServiceRequests.Dtos;

public class CreateServiceRequestDto
{
    public List<ServiceRequestItemDto> Items { get; set; } = new();
    public string? Remarks { get; set; }
}

public class ServiceRequestItemDto
{
    public Guid ItemId { get; set; }
    public int RequestedQty { get; set; }
    public Guid? PreferredShelfId { get; set; }
    public string? Notes { get; set; }
}