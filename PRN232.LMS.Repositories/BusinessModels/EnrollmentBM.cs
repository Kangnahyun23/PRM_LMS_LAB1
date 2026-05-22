namespace PRN232.LMS.Repositories.BusinessModels;

public sealed class EnrollmentBM
{
    public int EnrollmentId { get; set; }
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public DateTime EnrollDate { get; set; }
    public string Status { get; set; } = string.Empty;

    public StudentBM? Student { get; set; }
    public CourseBM? Course { get; set; }
}
