using PRN232.LMS.BusinessModels.Query;

namespace PRN232.LMS.Repositories.Interfaces;

public interface IRepository<TEntity>
{
    Task<TEntity?> GetByIdAsync(int id, QueryOptions? options = null, CancellationToken ct = default);
    Task<PagedResult<TEntity>> GetAllAsync(QueryOptions options, CancellationToken ct = default);
    Task<TEntity> CreateAsync(TEntity entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(TEntity entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}

