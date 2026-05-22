namespace PRN232.LMS.API.Models.Request;

public sealed class CourseRequest
{
    public string CourseName { get; set; } = string.Empty;
    public int SemesterId { get; set; }
}

