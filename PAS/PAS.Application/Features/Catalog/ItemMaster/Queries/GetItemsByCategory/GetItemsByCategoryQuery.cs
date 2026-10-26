using Application.Common.Security;
using Application.Features.Catalog.ItemMasters.Dtos;
using MediatR;

namespace Application.Features.Catalog.ItemMasters.Queries.GetItemsByCategory;

[Authorize(Permissions = Permissions.Items.View)]
public record GetItemsByCategoryQuery(Guid CategoryId) : IRequest<Result<List<ItemMasterListDto>>>;