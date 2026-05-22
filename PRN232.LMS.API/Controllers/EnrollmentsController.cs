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
public sealed class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _service;

    public EnrollmentsController(IEnrollmentService service)
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
    [ProducesResponseType(typeof(ApiResponse<EnrollmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken ct)
    {
        var enrollment = await _service.GetByIdAsync(id, ct);
        if (enrollment is null) return NotFound(ApiResponse<object>.Fail("Not found"));
        return Ok(ApiResponse<EnrollmentResponse>.Ok(MapToResponse(enrollment)));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentResponse>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] EnrollmentRequest request, CancellationToken ct)
    {
        var created = await _service.CreateAsync(new EnrollmentBM
        {
            StudentId = request.StudentId,
            CourseId = request.CourseId,
            EnrollDate = request.EnrollDate,
            Status = request.Status,
        }, ct);

        var response = MapToResponse(created);
        return CreatedAtAction(nameof(GetById), new { id = response.EnrollmentId }, ApiResponse<EnrollmentResponse>.Ok(response, "Created"));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] EnrollmentRequest request, CancellationToken ct)
    {
        var ok = await _service.UpdateAsync(id, new EnrollmentBM
        {
            EnrollmentId = id,
            StudentId = request.StudentId,
            CourseId = request.CourseId,
            EnrollDate = request.EnrollDate,
            Status = request.Status,
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

    private static EnrollmentResponse MapToResponse(EnrollmentBM bm) => new()
    {
        EnrollmentId = bm.EnrollmentId,
        StudentId = bm.StudentId,
        CourseId = bm.CourseId,
        EnrollDate = bm.EnrollDate,
        Status = bm.Status,
        Student = bm.Student is null
            ? null
            : new StudentResponse
            {
                StudentId = bm.Student.StudentId,
                FullName = bm.Student.FullName,
                Email = bm.Student.Email,
                DateOfBirth = bm.Student.DateOfBirth,
            },
        Course = bm.Course is null
            ? null
            : new CourseResponse
            {
                CourseId = bm.Course.CourseId,
                CourseName = bm.Course.CourseName,
                SemesterId = bm.Course.SemesterId,
                Semester = bm.Course.Semester is null
                    ? null
                    : new SemesterResponse
                    {
                        SemesterId = bm.Course.Semester.SemesterId,
                        SemesterName = bm.Course.Semester.SemesterName,
                        StartDate = bm.Course.Semester.StartDate,
                        EndDate = bm.Course.Semester.EndDate,
                    },
            },
    };
}

