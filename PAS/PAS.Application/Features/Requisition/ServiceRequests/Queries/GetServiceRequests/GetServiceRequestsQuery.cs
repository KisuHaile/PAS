using Application.Common.Security;
using Application.Features.Requisition.ServiceRequests.Dtos;
using MediatR;

namespace Application.Features.Requisition.ServiceRequests.Queries;

[Authorize(Permissions = Permissions.Requisitions.View)]
public record GetServiceRequestsQuery : IRequest<Result<PaginatedList<ServiceRequestListDto>>>
{
    public string? Status { get; init; }
    public Guid? RequesterId { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public string? Department { get; init; }
    public bool? MyRequests { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}