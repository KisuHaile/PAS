namespace PAS.API.Models.Responses;

public class PaginatedResponse<T>
{
    public IReadOnlyCollection<T> Items { get; set; } = new List<T>();
    public int PageNumber { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
}