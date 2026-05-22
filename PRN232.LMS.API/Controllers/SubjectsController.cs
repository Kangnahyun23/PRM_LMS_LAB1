using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Infrastructure;
using PRN232.LMS.API.Models.Request;
using PRN232.LMS.API.Models.Response;
using PRN232.LMS.BusinessModels;
using PRN232.LMS.BusinessModels.Query;
using PRN232.LMS.Services.Interfaces;

namespace PRN232.LMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class SubjectsController : ControllerBase
{
    private readonly ISubjectService _service;

    public SubjectsController(ISubjectService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<object>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] QueryOptions options, CancellationToken ct)
    {
        var paged = await _service.GetAllAsync(options, ct);
        var items = paged.Items.Select(MapToResponse).ToList();

        var shaped = FieldSelector.Shape(items, options.Fields).ToList();
        var response = new PagedResponse<object>
        {
            Items = shaped,
            Pagination = new PaginationMetaResponse
            {
                Page = paged.Pagination.Page,
                PageSize = paged.Pagination.PageSize,
                TotalItems = paged.Pagination.TotalItems,
                TotalPages = paged.Pagination.TotalPages,
            },
        };

        return Ok(ApiResponse<PagedResponse<object>>.Ok(response));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken ct)
    {
        var subject = await _service.GetByIdAsync(id, ct);
        if (subject is null) return NotFound(ApiResponse<object>.Fail("Not found"));
        return Ok(ApiResponse<SubjectResponse>.Ok(MapToResponse(subject)));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] SubjectRequest request, CancellationToken ct)
    {
        var created = await _service.CreateAsync(new SubjectBM
        {
            SubjectCode = request.SubjectCode,
            SubjectName = request.SubjectName,
            Credit = request.Credit,
        }, ct);

        var response = MapToResponse(created);
        return CreatedAtAction(nameof(GetById), new { id = response.SubjectId }, ApiResponse<SubjectResponse>.Ok(response, "Created"));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] SubjectRequest request, CancellationToken ct)
    {
        var ok = await _service.UpdateAsync(id, new SubjectBM
        {
            SubjectId = id,
            SubjectCode = request.SubjectCode,
            SubjectName = request.SubjectName,
            Credit = request.Credit,
        }, ct);

        if (!ok) return NotFound(ApiResponse<object>.Fail("Not found"));
        return Ok(ApiResponse<object>.Ok(new { updated = true }));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken ct)
    {
        var ok = await _service.DeleteAsync(id, ct);
        if (!ok) return NotFound(ApiResponse<object>.Fail("Not found"));
        return Ok(ApiResponse<object>.Ok(new { deleted = true }));
    }

    private static SubjectResponse MapToResponse(SubjectBM bm) => new()
    {
        SubjectId = bm.SubjectId,
        SubjectCode = bm.SubjectCode,
        SubjectName = bm.SubjectName,
        Credit = bm.Credit,
    };
}

