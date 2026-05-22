using PRN232.LMS.Repositories.Query;
using PRN232.LMS.Repositories.Models;

namespace PRN232.LMS.Repositories.Interfaces;

public interface ICourseRepository : IRepository<Course>
{
    Task<Course?> GetByIdWithRelatedAsync(int id, CancellationToken ct = default);
}

