namespace Application.Features.Catalog.Categories.Dtos;

public class CategoryDetailDto : CategoryDto
{
    public List<CategoryDto> SubCategories { get; set; } = new();
    public List<CategoryItemDto> Items { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CategoryItemDto
{
    public Guid Id { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string UnitOfMeasure { get; set; } = string.Empty;
    public int CurrentStock { get; set; }
}