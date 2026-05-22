using PRN232.LMS.BusinessModels;
using PRN232.LMS.BusinessModels.Query;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Repositories.Models;
using PRN232.LMS.Services.Interfaces;

namespace PRN232.LMS.Services.Implementations;

public sealed class SubjectService : ISubjectService
{
    private readonly ISubjectRepository _repo;

    public SubjectService(ISubjectRepository repo)
    {
        _repo = repo;
    }

    public async Task<SubjectBM?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(id, null, ct);
        return entity is null ? null : Map(entity);
    }

    public async Task<PagedResult<SubjectBM>> GetAllAsync(QueryOptions options, CancellationToken ct = default)
    {
        var paged = await _repo.GetAllAsync(options, ct);
        return new PagedResult<SubjectBM>
        {
            Items = paged.Items.Select(Map).ToList(),
            Pagination = paged.Pagination,
        };
    }

    public async Task<SubjectBM> CreateAsync(SubjectBM model, CancellationToken ct = default)
    {
        var entity = new Subject
        {
            SubjectCode = model.SubjectCode,
            SubjectName = model.SubjectName,
            Credit = model.Credit,
        };

        var created = await _repo.CreateAsync(entity, ct);
        return Map(created);
    }

    public async Task<bool> UpdateAsync(int id, SubjectBM model, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(id, null, ct);
        if (existing is null) return false;

        existing.SubjectCode = model.SubjectCode;
        existing.SubjectName = model.SubjectName;
        existing.Credit = model.Credit;
        return await _repo.UpdateAsync(existing, ct);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        => await _repo.DeleteAsync(id, ct);

    private static SubjectBM Map(Subject e) => new()
    {
        SubjectId = e.SubjectId,
        SubjectCode = e.SubjectCode,
        SubjectName = e.SubjectName,
        Credit = e.Credit,
    };
}

