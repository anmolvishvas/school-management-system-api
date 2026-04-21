namespace SchoolManagement.Application.Common.Models;

public class PagedResult<T>
{
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public IReadOnlyList<T> Data { get; set; } = Array.Empty<T>();
}
