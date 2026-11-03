using Application.Common.Security;
using Application.Features.PropertyManagement.SafetyBoxes.Dtos;
using MediatR;

namespace Application.Features.PropertyManagement.SafetyBoxes.Queries.GetSafetyBoxById;

[Authorize(Permissions = Permissions.SafetyBoxes.View)]
public record GetSafetyBoxByIdQuery(Guid Id) : IRequest<Result<SafetyBoxDetailDto>>;