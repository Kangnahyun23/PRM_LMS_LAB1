using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Infrastructure;
using PRN232.LMS.API.Models.Request;
using PRN232.LMS.API.Models.Response;
using PRN232.LMS.Repositories.BusinessModels;
using PRN232.LMS.Repositories.Query;
using PRN232.LMS.Services.Interfaces;

namespace PRN232.LMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class SemestersController : ControllerBase
{
    private readonly ISemesterService _service;

    public SemestersController(ISemesterService service)
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
    [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken ct)
    {
        var semester = await _service.GetByIdAsync(id, ct);
        if (semester is null) return NotFound(ApiResponse<object>.Fail("Not found"));
        return Ok(ApiResponse<SemesterResponse>.Ok(MapToResponse(semester)));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] SemesterRequest request, CancellationToken ct)
    {
        var created = await _service.CreateAsync(new SemesterBM
        {
            SemesterName = request.SemesterName,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
        }, ct);

        var response = MapToResponse(created);
        return CreatedAtAction(nameof(GetById), new { id = response.SemesterId }, ApiResponse<SemesterResponse>.Ok(response, "Created"));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] SemesterRequest request, CancellationToken ct)
    {
        var ok = await _service.UpdateAsync(id, new SemesterBM
        {
            SemesterId = id,
            SemesterName = request.SemesterName,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
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

    private static SemesterResponse MapToResponse(SemesterBM bm) => new()
    {
        SemesterId = bm.SemesterId,
        SemesterName = bm.SemesterName,
        StartDate = bm.StartDate,
        EndDate = bm.EndDate,
        Courses = bm.Courses is null
            ? null
            : bm.Courses.Select(c => new CourseResponse
            {
                CourseId = c.CourseId,
                CourseName = c.CourseName,
                SemesterId = c.SemesterId,
                Semester = null,
            }).ToList(),
    };
}

