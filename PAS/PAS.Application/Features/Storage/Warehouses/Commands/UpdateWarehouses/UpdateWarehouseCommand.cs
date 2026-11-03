using Application.Common.Security;
using Application.Events;
using MediatR;

namespace Application.Features.Storage.Warehouses.Commands;

[Authorize(Permissions = Permissions.Warehouses.Edit)]
public record UpdateWarehouseCommand : IRequest<Result>
{
    public Guid Id { get; init; }
    public string WarehouseName { get; init; } = string.Empty;
    public string LocationCode { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string ContactPerson { get; init; } = string.Empty;
    public string ContactPhone { get; init; } = string.Empty;
    public string ContactEmail { get; init; } = string.Empty;
    public bool IsActive { get; init; }
}