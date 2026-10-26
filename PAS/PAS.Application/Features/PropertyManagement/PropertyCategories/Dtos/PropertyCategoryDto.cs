using Application.Common.Mappings;
using AutoMapper;

namespace Application.Features.PropertyManagement.PropertyCategories.Dtos;

public class PropertyCategoryDto : IMapFrom<PropertyCategory>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int PropertiesCount { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<PropertyCategory, PropertyCategoryDto>()
            .ForMember(d => d.PropertiesCount, opt => opt.Ignore());
    }
}