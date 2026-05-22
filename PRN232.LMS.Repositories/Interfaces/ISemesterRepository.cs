using PRN232.LMS.Repositories.Query;
using PRN232.LMS.Repositories.Models;

namespace PRN232.LMS.Repositories.Interfaces;

public interface ISemesterRepository : IRepository<Semester>
{
    Task<Semester?> GetByIdWithRelatedAsync(int id, CancellationToken ct = default);
}

