using Application.Common.Mappings;
using AutoMapper;

namespace Application.Features.Catalog.ItemMasters.Dtos;

public class ItemMasterListDto : IMapFrom<ItemMaster>
{
    public Guid Id { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string UnitOfMeasure { get; set; } = string.Empty;
    public int CurrentStock { get; set; }
    public int ReservedStock { get; set; }
    public int AvailableStock { get; set; }
    public bool IsLowStock { get; set; }
    public decimal UnitPrice { get; set; }
    public string Status { get; set; } = "Active";

    public void Mapping(Profile profile)
    {
        profile.CreateMap<ItemMaster, ItemMasterListDto>()
            .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Category != null ? s.Category.Name : string.Empty))
            .ForMember(d => d.CurrentStock, opt => opt.Ignore())
            .ForMember(d => d.ReservedStock, opt => opt.Ignore())
            .ForMember(d => d.AvailableStock, opt => opt.Ignore())
            .ForMember(d => d.IsLowStock, opt => opt.Ignore());
    }
}