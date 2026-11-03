using Application.Common.Security;
using MediatR;

namespace Application.Features.PropertyManagement.SafetyBoxes.Commands.DeleteSafetyBox;

[Authorize(Permissions = Permissions.SafetyBoxes.Delete)]
public record DeleteSafetyBoxCommand(Guid Id) : IRequest<Result>;