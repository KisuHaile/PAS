using Application.Common.Security;
using Application.Features.Storage.InventoryStock.Dtos;
using MediatR;

namespace Application.Features.Storage.InventoryStock.Queries;

[Authorize(Permissions = Permissions.Inventory.View)]
public record GetStockByItemQuery(Guid ItemId) : IRequest<Result<StockByItemDto>>;