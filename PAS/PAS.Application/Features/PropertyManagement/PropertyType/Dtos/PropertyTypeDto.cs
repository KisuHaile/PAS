using Application.Common.Mappings;
using AutoMapper;

namespace Application.Features.PropertyManagement.PropertyTypes.Dtos;

public class PropertyTypeDto : IMapFrom<PropertyType>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int PropertiesCount { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<PropertyType, PropertyTypeDto>()
            .ForMember(d => d.PropertiesCount, opt => opt.Ignore());
    }
}