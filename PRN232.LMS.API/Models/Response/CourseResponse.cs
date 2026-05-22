namespace PRN232.LMS.API.Models.Response;

public sealed class CourseResponse
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int SemesterId { get; set; }

    public SemesterResponse? Semester { get; set; }
}

