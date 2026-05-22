using PRN232.LMS.Repositories.BusinessModels;
using PRN232.LMS.Repositories.Query;

namespace PRN232.LMS.Services.Interfaces;

public interface IEnrollmentService
{
    Task<EnrollmentBM?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<PagedResult<EnrollmentBM>> GetAllAsync(QueryOptions options, CancellationToken ct = default);
    Task<EnrollmentBM> CreateAsync(EnrollmentBM model, CancellationToken ct = default);
    Task<bool> UpdateAsync(int id, EnrollmentBM model, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}

