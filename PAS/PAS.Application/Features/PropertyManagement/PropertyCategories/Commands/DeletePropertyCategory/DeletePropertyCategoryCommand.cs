using Application.Common.Security;
using MediatR;

namespace Application.Features.PropertyManagement.PropertyCategories.Commands.DeletePropertyCategory;

[Authorize(Permissions = Permissions.PropertyCategories.Delete)]
public record DeletePropertyCategoryCommand(Guid Id) : IRequest<Result>;