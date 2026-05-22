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
public sealed class StudentsController : ControllerBase
{
    private readonly IStudentService _service;

    public StudentsController(IStudentService service)
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
    [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken ct)
    {
        var student = await _service.GetByIdAsync(id, ct);
        if (student is null)
        {
            return NotFound(ApiResponse<object>.Fail("Not found"));
        }

        return Ok(ApiResponse<StudentResponse>.Ok(MapToResponse(student)));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] StudentRequest request, CancellationToken ct)
    {
        var created = await _service.CreateAsync(new StudentBM
        {
            FullName = request.FullName,
            Email = request.Email,
            DateOfBirth = request.DateOfBirth,
        }, ct);

        var response = MapToResponse(created);
        return CreatedAtAction(nameof(GetById), new { id = response.StudentId }, ApiResponse<StudentResponse>.Ok(response, "Created"));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] StudentRequest request, CancellationToken ct)
    {
        var ok = await _service.UpdateAsync(id, new StudentBM
        {
            StudentId = id,
            FullName = request.FullName,
            Email = request.Email,
            DateOfBirth = request.DateOfBirth,
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

    private static StudentResponse MapToResponse(StudentBM bm) => new()
    {
        StudentId = bm.StudentId,
        FullName = bm.FullName,
        Email = bm.Email,
        DateOfBirth = bm.DateOfBirth,
        Enrollments = bm.Enrollments is null
            ? null
            : bm.Enrollments.Select(e => new EnrollmentResponse
            {
                EnrollmentId = e.EnrollmentId,
                StudentId = e.StudentId,
                CourseId = e.CourseId,
                EnrollDate = e.EnrollDate,
                Status = e.Status,
                Student = null,
                Course = e.Course is null
                    ? null
                    : new CourseResponse
                    {
                        CourseId = e.Course.CourseId,
                        CourseName = e.Course.CourseName,
                        SemesterId = e.Course.SemesterId,
                        Semester = e.Course.Semester is null
                            ? null
                            : new SemesterResponse
                            {
                                SemesterId = e.Course.Semester.SemesterId,
                                SemesterName = e.Course.Semester.SemesterName,
                                StartDate = e.Course.Semester.StartDate,
                                EndDate = e.Course.Semester.EndDate,
                            },
                    },
            }).ToList(),
    };
}

