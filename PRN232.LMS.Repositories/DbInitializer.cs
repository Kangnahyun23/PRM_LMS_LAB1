using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Models;

namespace PRN232.LMS.Repositories;

public static class DbInitializer
{
    public static async Task EnsureSeededAsync(LmsDbContext db, CancellationToken ct = default)
    {
        await db.Database.EnsureCreatedAsync(ct);

        if (await db.Semesters.AnyAsync(ct))
        {
            return;
        }

        var random = new Random(12345);

        var semesters = Enumerable.Range(1, 5)
            .Select(i =>
            {
                var start = DateTime.UtcNow.Date.AddMonths(4 * (i - 3));
                return new Semester
                {
                    SemesterName = $"Semester {i}",
                    StartDate = start,
                    EndDate = start.AddMonths(4).AddDays(-1),
                };
            })
            .ToList();
        db.Semesters.AddRange(semesters);
        await db.SaveChangesAsync(ct);

        var subjects = Enumerable.Range(1, 10)
            .Select(i => new Subject
            {
                SubjectCode = $"SUB{i:000}",
                SubjectName = $"Subject {i}",
                Credit = random.Next(1, 5),
            })
            .ToList();
        db.Subjects.AddRange(subjects);
        await db.SaveChangesAsync(ct);

        var courses = Enumerable.Range(1, 20)
            .Select(i => new Course
            {
                CourseName = $"Course {i}",
                SemesterId = semesters[random.Next(semesters.Count)].SemesterId,
            })
            .ToList();
        db.Courses.AddRange(courses);
        await db.SaveChangesAsync(ct);

        var students = Enumerable.Range(1, 50)
            .Select(i => new Student
            {
                FullName = $"Student {i}",
                Email = $"student{i}@example.com",
                DateOfBirth = DateTime.UtcNow.Date.AddYears(-random.Next(18, 26)).AddDays(random.Next(0, 365)),
            })
            .ToList();
        db.Students.AddRange(students);
        await db.SaveChangesAsync(ct);

        var statuses = new[] { "active", "inactive", "completed", "dropped" };

        var enrollments = Enumerable.Range(1, 500)
            .Select(_ => new Enrollment
            {
                StudentId = students[random.Next(students.Count)].StudentId,
                CourseId = courses[random.Next(courses.Count)].CourseId,
                EnrollDate = DateTime.UtcNow.Date.AddDays(-random.Next(0, 365)),
                Status = statuses[random.Next(statuses.Length)],
            })
            .ToList();
        db.Enrollments.AddRange(enrollments);

        await db.SaveChangesAsync(ct);
    }
}

