using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Query;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Repositories.Models;

namespace PRN232.LMS.Repositories.Implementations;

public sealed class SemesterRepository : BaseRepository<Semester>, ISemesterRepository
{
    public SemesterRepository(LmsDbContext db) : base(db)
    {
    }

    public async Task<Semester?> GetByIdWithRelatedAsync(int id, CancellationToken ct = default)
    {
        return await Db.Semesters
            .AsNoTracking()
            .Include(s => s.Courses)
            .FirstOrDefaultAsync(s => s.SemesterId == id, ct);
    }

    protected override IQueryable<Semester> ApplySearch(IQueryable<Semester> query, string? search)
    {
        if (string.IsNullOrWhiteSpace(search)) return query;
        search = search.Trim();
        return query.Where(s => s.SemesterName.Contains(search));
    }

    public override async Task<Semester?> GetByIdAsync(int id, QueryOptions? options = null, CancellationToken ct = default)
        => await GetByIdWithRelatedAsync(id, ct);

    public override async Task<PagedResult<Semester>> GetAllAsync(QueryOptions options, CancellationToken ct = default)
    {
        var expand = (options.Expand ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(x => x.ToLowerInvariant())
            .ToHashSet();

        IQueryable<Semester> query = Db.Semesters.AsNoTracking();
        if (expand.Contains("courses"))
        {
            query = query.Include(s => s.Courses);
        }

        query = ApplySearch(query, options.Search);
        query = query.ApplySort(options.Sort);
        return await query.ToPagedResultAsync(options.Page, options.Size, ct);
    }
}

