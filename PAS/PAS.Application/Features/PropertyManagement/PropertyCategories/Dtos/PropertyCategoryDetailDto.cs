namespace Application.Features.PropertyManagement.PropertyCategories.Dtos;

public class PropertyCategoryDetailDto : PropertyCategoryDto
{
    public List<PropertyCategoryPropertyDto> Properties { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class PropertyCategoryPropertyDto
{
    public Guid Id { get; set; }
    public string TagNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
}