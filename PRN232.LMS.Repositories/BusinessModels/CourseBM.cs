namespace PRN232.LMS.Repositories.BusinessModels;

public sealed class CourseBM
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int SemesterId { get; set; }

    public SemesterBM? Semester { get; set; }
}
