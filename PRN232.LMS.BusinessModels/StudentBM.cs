namespace PRN232.LMS.BusinessModels;

public sealed class StudentBM
{
    public int StudentId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }

    public IReadOnlyList<EnrollmentBM>? Enrollments { get; set; }
}

