using Application.Common.Mappings;
using AutoMapper;

namespace Application.Features.Storage.Warehouses.Dtos;

public class WarehouseDto : IMapFrom<Domain.Storage.Warehouse>
{
    public Guid Id { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public string LocationCode { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string ContactPerson { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int TotalShelves { get; set; }
    public int OccupiedShelves { get; set; }
    public int TotalItems { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Domain.Storage.Warehouse, WarehouseDto>()
            .ForMember(d => d.TotalShelves, opt => opt.Ignore())
            .ForMember(d => d.OccupiedShelves, opt => opt.Ignore())
            .ForMember(d => d.TotalItems, opt => opt.Ignore());
    }
}

public class WarehouseDetailDto : WarehouseDto
{
    public List<WarehouseShelfDto> Shelves { get; set; } = new();
    public List<WarehouseInventorySummaryDto> TopItems { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}

public class WarehouseShelfDto
{
    public Guid Id { get; set; }
    public string Aisle { get; set; } = string.Empty;
    public string Rack { get; set; } = string.Empty;
    public string ShelfNumber { get; set; } = string.Empty;
    public string FullLocation { get; set; } = string.Empty;
    public int ItemCount { get; set; }
    public int TotalQuantity { get; set; }
    public double UtilizationPercentage { get; set; }
}

public class WarehouseInventorySummaryDto
{
    public Guid ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public int ReservedQuantity { get; set; }
    public int AvailableQuantity => Quantity - ReservedQuantity;
}

public class WarehouseListDto
{
    public Guid Id { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public string LocationCode { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int ShelfCount { get; set; }
    public int ItemCount { get; set; }
}