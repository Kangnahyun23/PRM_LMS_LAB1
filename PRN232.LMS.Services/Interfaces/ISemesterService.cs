using PRN232.LMS.Repositories.BusinessModels;
using PRN232.LMS.Repositories.Query;

namespace PRN232.LMS.Services.Interfaces;

public interface ISemesterService
{
    Task<SemesterBM?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<PagedResult<SemesterBM>> GetAllAsync(QueryOptions options, CancellationToken ct = default);
    Task<SemesterBM> CreateAsync(SemesterBM model, CancellationToken ct = default);
    Task<bool> UpdateAsync(int id, SemesterBM model, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}

