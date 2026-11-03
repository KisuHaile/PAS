using Application.Common.Security;
using Application.Features.Storage.Warehouses.Dtos;
using MediatR;

namespace Application.Features.Storage.Warehouses.Queries;

[Authorize(Permissions = Permissions.Warehouses.View)]
public record GetWarehouseByIdQuery(Guid Id) : IRequest<Result<WarehouseDetailDto>>;