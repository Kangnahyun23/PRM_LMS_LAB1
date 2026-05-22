using PRN232.LMS.Repositories.BusinessModels;
using PRN232.LMS.Repositories.Query;

namespace PRN232.LMS.Services.Interfaces;

public interface ICourseService
{
    Task<CourseBM?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<PagedResult<CourseBM>> GetAllAsync(QueryOptions options, CancellationToken ct = default);
    Task<CourseBM> CreateAsync(CourseBM model, CancellationToken ct = default);
    Task<bool> UpdateAsync(int id, CourseBM model, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}

