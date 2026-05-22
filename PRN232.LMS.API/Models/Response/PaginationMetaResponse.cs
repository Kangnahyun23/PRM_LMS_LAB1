namespace PRN232.LMS.API.Models.Response;

public sealed class PaginationMetaResponse
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
}

