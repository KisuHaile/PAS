using Application.Common.Security;
using Application.Features.Requisition.ServiceRequests.Dtos;
using MediatR;

namespace Application.Features.Requisition.ServiceRequests.Queries;

[Authorize(Permissions = Permissions.Requisitions.View)]
public record GetServiceRequestByIdQuery(Guid Id) : IRequest<Result<ServiceRequestDetailDto>>;