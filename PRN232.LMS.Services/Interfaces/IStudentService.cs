using PRN232.LMS.Repositories.BusinessModels;
using PRN232.LMS.Repositories.Query;

namespace PRN232.LMS.Services.Interfaces;

public interface IStudentService
{
    Task<StudentBM?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<PagedResult<StudentBM>> GetAllAsync(QueryOptions options, CancellationToken ct = default);
    Task<StudentBM> CreateAsync(StudentBM model, CancellationToken ct = default);
    Task<bool> UpdateAsync(int id, StudentBM model, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}

