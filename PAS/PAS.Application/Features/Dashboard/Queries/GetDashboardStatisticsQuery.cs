using Application.Common.Security;
using Application.Features.Dashboard.Dtos;
using MediatR;

namespace Application.Features.Dashboard.Queries;

[Authorize(Permissions = Permissions.Dashboard.View)]
public record GetDashboardStatisticsQuery : IRequest<Result<DashboardDto>>;