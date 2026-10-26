namespace Application.Features.PropertyManagement.SafetyBoxes.Dtos;

public class SafetyBoxDetailDto : SafetyBoxDto
{
    public List<SafetyBoxShelfDto> Shelves { get; set; } = new();
    public List<SafetyBoxPropertyDto> Properties { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class SafetyBoxShelfDto
{
    public Guid Id { get; set; }
    public int ShelfNumber { get; set; }
    public int PropertiesCount { get; set; }
}

public class SafetyBoxPropertyDto
{
    public Guid Id { get; set; }
    public string TagNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int ShelfNumber { get; set; }
}