using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN232.LMS.Repositories.Models;

public class Semester
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int SemesterId { get; set; }

    [Required]
    [MaxLength(100)]
    public string SemesterName { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public ICollection<Course> Courses { get; set; } = new List<Course>();
}

