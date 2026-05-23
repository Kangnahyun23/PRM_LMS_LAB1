using PRN232.LMS.Repositories.BusinessModels;
using PRN232.LMS.Repositories.Query;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Repositories.Models;
using PRN232.LMS.Services.Interfaces;

namespace PRN232.LMS.Services.Implementations;

public sealed class StudentService : IStudentService
{
    private readonly IStudentRepository _repo;

    public StudentService(IStudentRepository repo)
    {
        _repo = repo;
    }

    public async Task<StudentBM?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdWithRelatedAsync(id, ct);
        if (entity is null) return null;

        return Map(entity);
    }

    public async Task<PagedResult<StudentBM>> GetAllAsync(QueryOptions options, CancellationToken ct = default)
    {
        var paged = await _repo.GetAllAsync(options, ct);
        return new PagedResult<StudentBM>
        {
            Items = paged.Items.Select(MapFlat).ToList(),
            Pagination = paged.Pagination,
        };
    }

    public async Task<StudentBM> CreateAsync(StudentBM model, CancellationToken ct = default)
    {
        var entity = new Student
        {
            FullName = model.FullName,
            Email = model.Email,
            DateOfBirth = model.DateOfBirth,
        };

        var created = await _repo.CreateAsync(entity, ct);
        return Map(created);
    }

    public async Task<bool> UpdateAsync(int id, StudentBM model, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(id, null, ct);
        if (existing is null) return false;

        existing.FullName = model.FullName;
        existing.Email = model.Email;
        existing.DateOfBirth = model.DateOfBirth;

        return await _repo.UpdateAsync(existing, ct);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        => await _repo.DeleteAsync(id, ct);

    // Used for list — no nested collections to avoid bloated responses
    private static StudentBM MapFlat(Student e) => new()
    {
        StudentId = e.StudentId,
        FullName = e.FullName,
        Email = e.Email,
        DateOfBirth = e.DateOfBirth,
    };

    // Used for GET by ID — includes full related data
    private static StudentBM Map(Student e) => new()
    {
        StudentId = e.StudentId,
        FullName = e.FullName,
        Email = e.Email,
        DateOfBirth = e.DateOfBirth,
        Enrollments = e.Enrollments is null
            ? null
            : e.Enrollments.Select(en => new EnrollmentBM
            {
                EnrollmentId = en.EnrollmentId,
                StudentId = en.StudentId,
                CourseId = en.CourseId,
                EnrollDate = en.EnrollDate,
                Status = en.Status,
                Course = en.Course is null
                    ? null
                    : new CourseBM
                    {
                        CourseId = en.Course.CourseId,
                        CourseName = en.Course.CourseName,
                        SemesterId = en.Course.SemesterId,
                        Semester = en.Course.Semester is null
                            ? null
                            : new SemesterBM
                            {
                                SemesterId = en.Course.Semester.SemesterId,
                                SemesterName = en.Course.Semester.SemesterName,
                                StartDate = en.Course.Semester.StartDate,
                                EndDate = en.Course.Semester.EndDate,
                            },
                    },
            }).ToList(),
    };
}

