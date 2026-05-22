namespace PRN232.LMS.Repositories.Query;

public sealed class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();
    public PaginationMeta Pagination { get; set; } = new();
}
