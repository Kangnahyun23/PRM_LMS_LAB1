using PRN232.LMS.BusinessModels.Query;
using PRN232.LMS.Repositories.Models;

namespace PRN232.LMS.Repositories.Interfaces;

public interface IStudentRepository : IRepository<Student>
{
    Task<Student?> GetByIdWithRelatedAsync(int id, CancellationToken ct = default);
}

