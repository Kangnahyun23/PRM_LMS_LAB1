namespace PRN232.LMS.API.Models.Response;

public sealed class SemesterResponse
{
    public int SemesterId { get; set; }
    public string SemesterName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public IReadOnlyList<CourseResponse>? Courses { get; set; }
}

