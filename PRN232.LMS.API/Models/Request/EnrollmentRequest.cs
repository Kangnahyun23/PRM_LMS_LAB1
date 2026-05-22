namespace PRN232.LMS.API.Models.Request;

public sealed class EnrollmentRequest
{
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public DateTime EnrollDate { get; set; }
    public string Status { get; set; } = "active";
}

