using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Query;
using PRN232.LMS.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.Implementations;

public abstract class BaseRepository<TEntity> : IRepository<TEntity>
    where TEntity : class
{
    protected readonly LmsDbContext Db;
    protected readonly DbSet<TEntity> Set;

    protected BaseRepository(LmsDbContext db)
    {
        Db = db;
        Set = db.Set<TEntity>();
    }

    protected virtual IQueryable<TEntity> ApplyIncludes(IQueryable<TEntity> query, QueryOptions? options) => query;

    protected virtual IQueryable<TEntity> ApplySearch(IQueryable<TEntity> query, string? search) => query;

    public virtual async Task<TEntity?> GetByIdAsync(int id, QueryOptions? options = null, CancellationToken ct = default)
    {
        // Derived repositories override when they need includes for GET by id
        return await Set.FindAsync(new object?[] { id }, cancellationToken: ct);
    }

    public virtual async Task<PagedResult<TEntity>> GetAllAsync(QueryOptions options, CancellationToken ct = default)
    {
        var query = ApplyIncludes(Set.AsNoTracking(), options);
        query = ApplySearch(query, options.Search);
        query = query.ApplySort(options.Sort);

        return await query.ToPagedResultAsync(options.Page, options.Size, ct);
    }

    public virtual async Task<TEntity> CreateAsync(TEntity entity, CancellationToken ct = default)
    {
        Set.Add(entity);
        await Db.SaveChangesAsync(ct);
        return entity;
    }

    public virtual async Task<bool> UpdateAsync(TEntity entity, CancellationToken ct = default)
    {
        Set.Update(entity);
        return await Db.SaveChangesAsync(ct) > 0;
    }

    public virtual async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var existing = await Set.FindAsync(new object?[] { id }, cancellationToken: ct);
        if (existing is null) return false;

        Set.Remove(existing);
        return await Db.SaveChangesAsync(ct) > 0;
    }
}

