using Microsoft.EntityFrameworkCore;
using PRN232.LMS.BusinessModels.Query;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Repositories.Models;
using PRN232.LMS.Repositories.Query;

namespace PRN232.LMS.Repositories.Implementations;

public sealed class EnrollmentRepository : BaseRepository<Enrollment>, IEnrollmentRepository
{
    public EnrollmentRepository(LmsDbContext db) : base(db)
    {
    }

    public async Task<Enrollment?> GetByIdWithRelatedAsync(int id, CancellationToken ct = default)
    {
        return await Db.Enrollments
            .AsNoTracking()
            .Include(e => e.Student)
            .Include(e => e.Course)
            .ThenInclude(c => c!.Semester)
            .FirstOrDefaultAsync(e => e.EnrollmentId == id, ct);
    }

    protected override IQueryable<Enrollment> ApplySearch(IQueryable<Enrollment> query, string? search)
    {
        if (string.IsNullOrWhiteSpace(search)) return query;
        search = search.Trim();

        // Search by status; if expand includes student/course then services can filter further.
        return query.Where(e => e.Status.Contains(search));
    }

    public override async Task<Enrollment?> GetByIdAsync(int id, QueryOptions? options = null, CancellationToken ct = default)
        => await GetByIdWithRelatedAsync(id, ct);

    public override async Task<PagedResult<Enrollment>> GetAllAsync(QueryOptions options, CancellationToken ct = default)
    {
        var expand = (options.Expand ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(x => x.ToLowerInvariant())
            .ToHashSet();

        IQueryable<Enrollment> query = Db.Enrollments.AsNoTracking();
        if (expand.Contains("student"))
        {
            query = query.Include(e => e.Student);
        }

        if (expand.Contains("course"))
        {
            query = query.Include(e => e.Course).ThenInclude(c => c!.Semester);
        }

        query = ApplySearch(query, options.Search);
        query = query.ApplySort(options.Sort);
        return await query.ToPagedResultAsync(options.Page, options.Size, ct);
    }
}

