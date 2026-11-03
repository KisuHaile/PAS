using Application.Common.Security;
using Application.Features.Catalog.ItemMasters.Dtos;
using MediatR;

namespace Application.Features.Catalog.ItemMasters.Queries.GetItemMasterById;

[Authorize(Permissions = Permissions.Items.View)]
public record GetItemMasterByIdQuery(Guid Id) : IRequest<Result<ItemMasterDetailDto>>;