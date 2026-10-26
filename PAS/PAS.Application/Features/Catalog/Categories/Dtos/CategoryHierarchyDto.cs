namespace Application.Features.Catalog.Categories.Dtos;

public class CategoryHierarchyDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<CategoryHierarchyDto> Children { get; set; } = new();
}