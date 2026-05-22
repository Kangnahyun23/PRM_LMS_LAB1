using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Query;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Repositories.Models;

namespace PRN232.LMS.Repositories.Implementations;

public sealed class SubjectRepository : BaseRepository<Subject>, ISubjectRepository
{
    public SubjectRepository(LmsDbContext db) : base(db)
    {
    }

    protected override IQueryable<Subject> ApplySearch(IQueryable<Subject> query, string? search)
    {
        if (string.IsNullOrWhiteSpace(search)) return query;
        search = search.Trim();
        return query.Where(s => s.SubjectCode.Contains(search) || s.SubjectName.Contains(search));
    }

    public override async Task<PagedResult<Subject>> GetAllAsync(QueryOptions options, CancellationToken ct = default)
    {
        var query = Db.Subjects.AsNoTracking();
        query = ApplySearch(query, options.Search);
        query = query.ApplySort(options.Sort);
        return await query.ToPagedResultAsync(options.Page, options.Size, ct);
    }
}

