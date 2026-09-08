using AdoNet.ThreeLayer.BLL.Students;
using Microsoft.AspNetCore.Mvc;

namespace AdoNet.ThreeLayer.Api.Controllers;

/// <summary>
/// HTTP-шар. Тільки приймає запити й повертає коди відповіді —
/// уся робота делегується у <see cref="IStudentService"/> (шар BLL).
/// Маршрути повторюють базовий приклад AdoNet-WebApi-Ex-01.
/// </summary>
[ApiController]
[Route("Students")]
public sealed class StudentsController : ControllerBase
{
    private readonly IStudentService _service;

    public StudentsController(IStudentService service)
    {
        _service = service;
    }

    // GET /Students?page=1&count=10
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StudentDto>>> GetStudents(int page = 1, int count = 10)
    {
        if (page <= 0 || count <= 0)
        {
            return BadRequest("page та count мають бути більші за 0.");
        }

        return Ok(await _service.GetStudentsAsync(page, count));
    }

    // GET /Students/Count
    [HttpGet("Count")]
    public async Task<ActionResult<int>> GetStudentsCount()
    {
        return Ok(await _service.GetStudentsCountAsync());
    }

    // GET /Students/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<StudentDto>> GetStudent(int id)
    {
        var student = await _service.GetStudentAsync(id);
        return student is null ? NotFound() : Ok(student);
    }

    // POST /Students/AddStudent
    [HttpPost("AddStudent")]
    public async Task<IActionResult> AddStudent([FromBody] CreateStudentDto student)
    {
        try
        {
            await _service.AddStudentAsync(student);
            return StatusCode(StatusCodes.Status201Created);
        }
        catch (ValidationException ex)
        {
            // Бізнес-правило не пройдено -> 400 Bad Request з поясненням.
            return BadRequest(ex.Message);
        }
    }

    // DELETE /Students/DeleteStudents/5
    [HttpDelete("DeleteStudents/{id:int}")]
    public async Task<IActionResult> DeleteStudent(int id)
    {
        bool deleted = await _service.DeleteStudentAsync(id);
        return deleted ? Ok("Deleted Successfully") : NotFound();
    }
}
