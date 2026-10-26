using Application.Common.Mappings;
using AutoMapper;

namespace Application.Features.PropertyManagement.Locations.Dtos;

public class LocationDto : IMapFrom<PropertyLocation>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LocationType { get; set; } = string.Empty;
    public int PropertiesCount { get; set; }
    public int SafetyBoxesCount { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<PropertyLocation, LocationDto>()
            .ForMember(d => d.PropertiesCount, opt => opt.Ignore())
            .ForMember(d => d.SafetyBoxesCount, opt => opt.Ignore());
    }
}