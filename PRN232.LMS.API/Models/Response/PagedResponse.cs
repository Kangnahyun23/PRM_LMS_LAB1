namespace PRN232.LMS.API.Models.Response;

public sealed class PagedResponse<T>
{
    public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();
    public PaginationMetaResponse Pagination { get; set; } = new();
}

