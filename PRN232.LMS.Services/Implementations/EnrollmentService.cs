using PRN232.LMS.Repositories.BusinessModels;
using PRN232.LMS.Repositories.Query;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Repositories.Models;
using PRN232.LMS.Services.Interfaces;

namespace PRN232.LMS.Services.Implementations;

public sealed class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository _repo;

    public EnrollmentService(IEnrollmentRepository repo)
    {
        _repo = repo;
    }

    public async Task<EnrollmentBM?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdWithRelatedAsync(id, ct);
        if (entity is null) return null;
        return Map(entity);
    }

    public async Task<PagedResult<EnrollmentBM>> GetAllAsync(QueryOptions options, CancellationToken ct = default)
    {
        var paged = await _repo.GetAllAsync(options, ct);
        return new PagedResult<EnrollmentBM>
        {
            Items = paged.Items.Select(Map).ToList(),
            Pagination = paged.Pagination,
        };
    }

    public async Task<EnrollmentBM> CreateAsync(EnrollmentBM model, CancellationToken ct = default)
    {
        var entity = new Enrollment
        {
            StudentId = model.StudentId,
            CourseId = model.CourseId,
            EnrollDate = model.EnrollDate,
            Status = model.Status,
        };

        var created = await _repo.CreateAsync(entity, ct);
        return Map(created);
    }

    public async Task<bool> UpdateAsync(int id, EnrollmentBM model, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(id, null, ct);
        if (existing is null) return false;

        existing.StudentId = model.StudentId;
        existing.CourseId = model.CourseId;
        existing.EnrollDate = model.EnrollDate;
        existing.Status = model.Status;

        return await _repo.UpdateAsync(existing, ct);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        => await _repo.DeleteAsync(id, ct);

    private static EnrollmentBM Map(Enrollment e) => new()
    {
        EnrollmentId = e.EnrollmentId,
        StudentId = e.StudentId,
        CourseId = e.CourseId,
        EnrollDate = e.EnrollDate,
        Status = e.Status,
        Student = e.Student is null
            ? null
            : new StudentBM
            {
                StudentId = e.Student.StudentId,
                FullName = e.Student.FullName,
                Email = e.Student.Email,
                DateOfBirth = e.Student.DateOfBirth,
            },
        Course = e.Course is null
            ? null
            : new CourseBM
            {
                CourseId = e.Course.CourseId,
                CourseName = e.Course.CourseName,
                SemesterId = e.Course.SemesterId,
                Semester = e.Course.Semester is null
                    ? null
                    : new SemesterBM
                    {
                        SemesterId = e.Course.Semester.SemesterId,
                        SemesterName = e.Course.Semester.SemesterName,
                        StartDate = e.Course.Semester.StartDate,
                        EndDate = e.Course.Semester.EndDate,
                    },
            },
    };
}

