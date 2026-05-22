using PRN232.LMS.Repositories.Query;
using PRN232.LMS.Repositories.Models;

namespace PRN232.LMS.Repositories.Interfaces;

public interface IEnrollmentRepository : IRepository<Enrollment>
{
    Task<Enrollment?> GetByIdWithRelatedAsync(int id, CancellationToken ct = default);
}

