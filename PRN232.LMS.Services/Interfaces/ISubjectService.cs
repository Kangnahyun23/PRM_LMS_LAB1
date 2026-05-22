using PRN232.LMS.BusinessModels;
using PRN232.LMS.BusinessModels.Query;

namespace PRN232.LMS.Services.Interfaces;

public interface ISubjectService
{
    Task<SubjectBM?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<PagedResult<SubjectBM>> GetAllAsync(QueryOptions options, CancellationToken ct = default);
    Task<SubjectBM> CreateAsync(SubjectBM model, CancellationToken ct = default);
    Task<bool> UpdateAsync(int id, SubjectBM model, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}

