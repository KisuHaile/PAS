using Application.Common.Security;
using MediatR;

namespace Application.Features.Catalog.Categories.Commands.DeleteCategory;

[Authorize(Permissions = Permissions.Categories.Delete)]
public record DeleteCategoryCommand(Guid Id) : IRequest<Result>;