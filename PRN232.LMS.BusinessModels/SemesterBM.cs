namespace PRN232.LMS.BusinessModels;

public sealed class SemesterBM
{
    public int SemesterId { get; set; }
    public string SemesterName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public IReadOnlyList<CourseBM>? Courses { get; set; }
}

