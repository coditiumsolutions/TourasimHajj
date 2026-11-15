namespace TourismHajj.ViewModels;

public class PagedResult<T>
{
    public IList<T> Items { get; set; } = new List<T>();
    public int PageIndex { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public int PageSize { get; set; }
    public string? Search { get; set; }

    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;
}