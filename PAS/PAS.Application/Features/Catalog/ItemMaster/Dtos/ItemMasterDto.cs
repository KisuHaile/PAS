using Application.Common.Mappings;
using AutoMapper;

namespace Application.Features.Catalog.ItemMasters.Dtos;

public class ItemMasterDto : IMapFrom<ItemMaster>
{
    public Guid Id { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string UnitOfMeasure { get; set; } = string.Empty;
    public bool RequiresInspection { get; set; }
    public int MinStockLevel { get; set; }
    public int TotalStock { get; set; }
    public int AvailableStock { get; set; }
    public decimal UnitPrice { get; set; }
    public string Status { get; set; } = "Active";

    public void Mapping(Profile profile)
    {
        profile.CreateMap<ItemMaster, ItemMasterDto>()
            .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Category != null ? s.Category.Name : string.Empty))
            .ForMember(d => d.TotalStock, opt => opt.Ignore())
            .ForMember(d => d.AvailableStock, opt => opt.Ignore());
    }
}