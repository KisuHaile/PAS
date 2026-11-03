using Application.Common.Mappings;
using AutoMapper;

namespace Application.Features.PropertyManagement.SafetyBoxes.Dtos;

public class SafetyBoxDto : IMapFrom<SafetyBox>
{
    public Guid Id { get; set; }
    public string BoxNumber { get; set; } = string.Empty;
    public int TotalShelves { get; set; }
    public Guid LocationId { get; set; }
    public string LocationName { get; set; } = string.Empty;
    public int OccupiedShelves { get; set; }
    public int PropertiesCount { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<SafetyBox, SafetyBoxDto>()
            .ForMember(d => d.LocationName, opt => opt.MapFrom(s => s.Location != null ? s.Location.Name : string.Empty))
            .ForMember(d => d.OccupiedShelves, opt => opt.Ignore())
            .ForMember(d => d.PropertiesCount, opt => opt.Ignore());
    }
}