using Microsoft.EntityFrameworkCore;
using PRN232.LMS.BusinessModels.Query;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Repositories.Models;
using PRN232.LMS.Repositories.Query;

namespace PRN232.LMS.Repositories.Implementations;

public sealed class StudentRepository : BaseRepository<Student>, IStudentRepository
{
    public StudentRepository(LmsDbContext db) : base(db)
    {
    }

    public async Task<Student?> GetByIdWithRelatedAsync(int id, CancellationToken ct = default)
    {
        return await Db.Students
            .AsNoTracking()
            .Include(s => s.Enrollments)
            .ThenInclude(e => e.Course)
            .ThenInclude(c => c!.Semester)
            .FirstOrDefaultAsync(s => s.StudentId == id, ct);
    }

    protected override IQueryable<Student> ApplySearch(IQueryable<Student> query, string? search)
    {
        if (string.IsNullOrWhiteSpace(search)) return query;
        search = search.Trim();
        return query.Where(s => s.FullName.Contains(search) || s.Email.Contains(search));
    }

    public override async Task<Student?> GetByIdAsync(int id, QueryOptions? options = null, CancellationToken ct = default)
        => await GetByIdWithRelatedAsync(id, ct);

    public override async Task<PagedResult<Student>> GetAllAsync(QueryOptions options, CancellationToken ct = default)
    {
        var query = Db.Students.AsNoTracking();
        query = ApplySearch(query, options.Search);
        query = query.ApplySort(options.Sort);
        return await query.ToPagedResultAsync(options.Page, options.Size, ct);
    }
}

