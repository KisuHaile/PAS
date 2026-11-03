using Application.Common.Mappings;
using AutoMapper;

namespace Application.Features.Catalog.Categories.Dtos;

public class CategoryDto : IMapFrom<Category>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid? ParentCategoryId { get; set; }
    public string? ParentCategoryName { get; set; }
    public int SubCategoriesCount { get; set; }
    public int ItemsCount { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Category, CategoryDto>()
            .ForMember(d => d.ParentCategoryName, opt => opt.Ignore())
            .ForMember(d => d.SubCategoriesCount, opt => opt.Ignore())
            .ForMember(d => d.ItemsCount, opt => opt.Ignore());
    }
}