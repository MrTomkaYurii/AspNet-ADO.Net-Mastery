using AdoNet.ThreeLayer.BLL.Students;
using Microsoft.AspNetCore.Mvc;

namespace AdoNet.ThreeLayer.Api.Controllers;

/// <summary>
/// HTTP-шар. Тільки приймає запити й повертає коди відповіді —
/// уся робота делегується у <see cref="IStudentService"/> (шар BLL).
/// Маршрути повторюють базовий приклад AdoNet-WebApi-Ex-01.
/// </summary>
/// <remarks>
/// <b>Архітектурна відповідальність контролера:</b>
/// Контролер виступає адаптером між веб-протоколом HTTP та сервісами програми.
/// Він відповідає за серіалізацію/десеріалізацію JSON, перевірку коректності запитів
/// та повернення статус-кодів за специфікацією REST: 200, 201, 400, 404.
/// </remarks>
[ApiController]
[Route("Students")]
public sealed class StudentsController : ControllerBase
{
    private readonly IStudentService _service;

    public StudentsController(IStudentService service)
    {
        _service = service;
    }

    /// <summary>Отримує пагінований список студентів.</summary>
    /// <param name="page">Номер сторінки (починаючи з 1).</param>
    /// <param name="count">Кількість елементів на сторінці.</param>
    /// <response code="200">Успішне отримання сторінки студентів.</response>
    /// <response code="400">Некоректні параметри пагінації.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<StudentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<StudentDto>>> GetStudents(int page = 1, int count = 10)
    {
        if (page <= 0 || count <= 0)
        {
            return BadRequest("page та count мають бути більші за 0.");
        }

        return Ok(await _service.GetStudentsAsync(page, count));
    }

    /// <summary>Отримує загальну кількість зареєстрованих студентів.</summary>
    /// <response code="200">Загальне число студентів.</response>
    [HttpGet("Count")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<ActionResult<int>> GetStudentsCount()
    {
        return Ok(await _service.GetStudentsCountAsync());
    }

    /// <summary>Отримує профіль студента за його унікальним ідентифікатором.</summary>
    /// <param name="id">Ідентифікатор студента.</param>
    /// <response code="200">Студента знайдено.</response>
    /// <response code="404">Студента з вказаним Id не існує.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(StudentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StudentDto>> GetStudent(int id)
    {
        var student = await _service.GetStudentAsync(id);
        return student is null ? NotFound() : Ok(student);
    }

    /// <summary>Реєструє нового студента в системі.</summary>
    /// <param name="student">Дані нового студента (ім'я та email).</param>
    /// <response code="201">Студента успішно створено.</response>
    /// <response code="400">Помилка валідації бізнес-правил (порожнє ім'я, некоректний email).</response>
    [HttpPost("AddStudent")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

    /// <summary>Видаляє студента за його ідентифікатором.</summary>
    /// <param name="id">Ідентифікатор студента.</param>
    /// <response code="200">Студента успішно видалено.</response>
    /// <response code="404">Студента для видалення не знайдено.</response>
    [HttpDelete("DeleteStudents/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteStudent(int id)
    {
        bool deleted = await _service.DeleteStudentAsync(id);
        return deleted ? Ok("Deleted Successfully") : NotFound();
    }
}
