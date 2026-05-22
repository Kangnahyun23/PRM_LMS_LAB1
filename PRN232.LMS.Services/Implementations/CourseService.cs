using PRN232.LMS.BusinessModels;
using PRN232.LMS.BusinessModels.Query;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Repositories.Models;
using PRN232.LMS.Services.Interfaces;

namespace PRN232.LMS.Services.Implementations;

public sealed class CourseService : ICourseService
{
    private readonly ICourseRepository _repo;

    public CourseService(ICourseRepository repo)
    {
        _repo = repo;
    }

    public async Task<CourseBM?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdWithRelatedAsync(id, ct);
        return entity is null ? null : Map(entity);
    }

    public async Task<PagedResult<CourseBM>> GetAllAsync(QueryOptions options, CancellationToken ct = default)
    {
        var paged = await _repo.GetAllAsync(options, ct);
        return new PagedResult<CourseBM>
        {
            Items = paged.Items.Select(Map).ToList(),
            Pagination = paged.Pagination,
        };
    }

    public async Task<CourseBM> CreateAsync(CourseBM model, CancellationToken ct = default)
    {
        var entity = new Course
        {
            CourseName = model.CourseName,
            SemesterId = model.SemesterId,
        };

        var created = await _repo.CreateAsync(entity, ct);
        return Map(created);
    }

    public async Task<bool> UpdateAsync(int id, CourseBM model, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(id, null, ct);
        if (existing is null) return false;

        existing.CourseName = model.CourseName;
        existing.SemesterId = model.SemesterId;
        return await _repo.UpdateAsync(existing, ct);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        => await _repo.DeleteAsync(id, ct);

    private static CourseBM Map(Course e) => new()
    {
        CourseId = e.CourseId,
        CourseName = e.CourseName,
        SemesterId = e.SemesterId,
        Semester = e.Semester is null
            ? null
            : new SemesterBM
            {
                SemesterId = e.Semester.SemesterId,
                SemesterName = e.Semester.SemesterName,
                StartDate = e.Semester.StartDate,
                EndDate = e.Semester.EndDate,
            },
    };
}

