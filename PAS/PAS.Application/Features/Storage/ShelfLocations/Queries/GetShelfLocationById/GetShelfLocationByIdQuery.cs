using Application.Common.Security;
using Application.Features.Storage.ShelfLocations.Dtos;
using MediatR;

namespace Application.Features.Storage.ShelfLocations.Queries;

[Authorize(Permissions = Permissions.ShelfLocations.View)]
public record GetShelfLocationByIdQuery(Guid Id) : IRequest<Result<ShelfLocationDetailDto>>;