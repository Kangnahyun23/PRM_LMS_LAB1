namespace PRN232.LMS.API.Models.Response;

public sealed class StudentResponse
{
    public int StudentId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }

    public IReadOnlyList<EnrollmentResponse>? Enrollments { get; set; }
}

