using Application.Common.Security;
using MediatR;

namespace Application.Features.Catalog.ItemMasters.Commands.DeleteItemMaster;

[Authorize(Permissions = Permissions.Items.Delete)]
public record DeleteItemMasterCommand(Guid Id) : IRequest<Result>;