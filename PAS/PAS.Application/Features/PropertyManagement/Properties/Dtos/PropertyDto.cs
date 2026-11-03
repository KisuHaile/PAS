using Application.Common.Mappings;
using AutoMapper;

namespace Application.Features.PropertyManagement.Properties.Dtos;

public class PropertyDto : IMapFrom<Domain.PropertyManagement.Property>
{
    public Guid Id { get; set; }
    public string TagNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public Guid PropertyTypeId { get; set; }
    public string PropertyTypeName { get; set; } = string.Empty;
    public Guid? PropertyCategoryId { get; set; }
    public string PropertyCategoryName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public decimal TotalValue => UnitPrice * Quantity;
    public int Quantity { get; set; }
    public DateTime PurchaseDate { get; set; }
    public Guid LocationId { get; set; }
    public string LocationName { get; set; } = string.Empty;
    public Guid? SafetyBoxId { get; set; }
    public string? SafetyBoxNumber { get; set; }
    public int? ShelfNumber { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Domain.PropertyManagement.Property, PropertyDto>()
            .ForMember(d => d.PropertyTypeName, opt => opt.MapFrom(s => s.PropertyType != null ? s.PropertyType.Name : string.Empty))
            .ForMember(d => d.PropertyCategoryName, opt => opt.MapFrom(s => s.PropertyCategory != null ? s.PropertyCategory.Name : string.Empty))
            .ForMember(d => d.LocationName, opt => opt.MapFrom(s => s.Location != null ? s.Location.Name : string.Empty))
            .ForMember(d => d.SafetyBoxNumber, opt => opt.MapFrom(s => s.SafetyBox != null ? s.SafetyBox.BoxNumber : null));
    }
}