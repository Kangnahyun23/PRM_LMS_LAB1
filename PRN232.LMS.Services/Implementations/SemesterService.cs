using PRN232.LMS.BusinessModels;
using PRN232.LMS.BusinessModels.Query;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Repositories.Models;
using PRN232.LMS.Services.Interfaces;

namespace PRN232.LMS.Services.Implementations;

public sealed class SemesterService : ISemesterService
{
    private readonly ISemesterRepository _repo;

    public SemesterService(ISemesterRepository repo)
    {
        _repo = repo;
    }

    public async Task<SemesterBM?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdWithRelatedAsync(id, ct);
        return entity is null ? null : Map(entity);
    }

    public async Task<PagedResult<SemesterBM>> GetAllAsync(QueryOptions options, CancellationToken ct = default)
    {
        var paged = await _repo.GetAllAsync(options, ct);
        return new PagedResult<SemesterBM>
        {
            Items = paged.Items.Select(Map).ToList(),
            Pagination = paged.Pagination,
        };
    }

    public async Task<SemesterBM> CreateAsync(SemesterBM model, CancellationToken ct = default)
    {
        var entity = new Semester
        {
            SemesterName = model.SemesterName,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
        };

        var created = await _repo.CreateAsync(entity, ct);
        return Map(created);
    }

    public async Task<bool> UpdateAsync(int id, SemesterBM model, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(id, null, ct);
        if (existing is null) return false;

        existing.SemesterName = model.SemesterName;
        existing.StartDate = model.StartDate;
        existing.EndDate = model.EndDate;
        return await _repo.UpdateAsync(existing, ct);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        => await _repo.DeleteAsync(id, ct);

    private static SemesterBM Map(Semester e) => new()
    {
        SemesterId = e.SemesterId,
        SemesterName = e.SemesterName,
        StartDate = e.StartDate,
        EndDate = e.EndDate,
        Courses = e.Courses is null
            ? null
            : e.Courses.Select(c => new CourseBM
            {
                CourseId = c.CourseId,
                CourseName = c.CourseName,
                SemesterId = c.SemesterId,
            }).ToList(),
    };
}

