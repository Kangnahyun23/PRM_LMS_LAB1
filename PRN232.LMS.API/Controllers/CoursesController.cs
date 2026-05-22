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
public sealed class CoursesController : ControllerBase
{
    private readonly ICourseService _service;

    public CoursesController(ICourseService service)
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
    [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken ct)
    {
        var course = await _service.GetByIdAsync(id, ct);
        if (course is null) return NotFound(ApiResponse<object>.Fail("Not found"));
        return Ok(ApiResponse<CourseResponse>.Ok(MapToResponse(course)));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CourseRequest request, CancellationToken ct)
    {
        var created = await _service.CreateAsync(new CourseBM
        {
            CourseName = request.CourseName,
            SemesterId = request.SemesterId,
        }, ct);

        var response = MapToResponse(created);
        return CreatedAtAction(nameof(GetById), new { id = response.CourseId }, ApiResponse<CourseResponse>.Ok(response, "Created"));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] CourseRequest request, CancellationToken ct)
    {
        var ok = await _service.UpdateAsync(id, new CourseBM
        {
            CourseId = id,
            CourseName = request.CourseName,
            SemesterId = request.SemesterId,
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

    private static CourseResponse MapToResponse(CourseBM bm) => new()
    {
        CourseId = bm.CourseId,
        CourseName = bm.CourseName,
        SemesterId = bm.SemesterId,
        Semester = bm.Semester is null
            ? null
            : new SemesterResponse
            {
                SemesterId = bm.Semester.SemesterId,
                SemesterName = bm.Semester.SemesterName,
                StartDate = bm.Semester.StartDate,
                EndDate = bm.Semester.EndDate,
            },
    };
}

