using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Query;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Repositories.Models;

namespace PRN232.LMS.Repositories.Implementations;

public sealed class CourseRepository : BaseRepository<Course>, ICourseRepository
{
    public CourseRepository(LmsDbContext db) : base(db)
    {
    }

    public async Task<Course?> GetByIdWithRelatedAsync(int id, CancellationToken ct = default)
    {
        return await Db.Courses
            .AsNoTracking()
            .Include(c => c.Semester)
            .FirstOrDefaultAsync(c => c.CourseId == id, ct);
    }

    protected override IQueryable<Course> ApplySearch(IQueryable<Course> query, string? search)
    {
        if (string.IsNullOrWhiteSpace(search)) return query;
        search = search.Trim();
        return query.Where(c => c.CourseName.Contains(search));
    }

    public override async Task<Course?> GetByIdAsync(int id, QueryOptions? options = null, CancellationToken ct = default)
        => await GetByIdWithRelatedAsync(id, ct);

    public override async Task<PagedResult<Course>> GetAllAsync(QueryOptions options, CancellationToken ct = default)
    {
        var expand = (options.Expand ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(x => x.ToLowerInvariant())
            .ToHashSet();

        IQueryable<Course> query = Db.Courses.AsNoTracking();
        if (expand.Contains("semester"))
        {
            query = query.Include(c => c.Semester);
        }

        query = ApplySearch(query, options.Search);
        query = query.ApplySort(options.Sort);
        return await query.ToPagedResultAsync(options.Page, options.Size, ct);
    }
}

