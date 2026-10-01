using Microsoft.AspNetCore.Mvc;

namespace LearnAdoDotnet.Controllers
{
    /// <summary>
    /// Контролер студентів для базового прикладу Example 02.
    /// Демонструє початковий етап розвитку архітектури: звернення до бізнес-сервісу,
    /// який використовує простий ADO.NET репозиторій.
    /// </summary>
    /// <remarks>
    /// <b>Порівняння з Clean Architecture (Example 03 та 04):</b>
    /// У цьому прикладі сервіси ще мають зв'язаність з реалізацією даних та синхронними викликами.
    /// Для перегляду сучасного стандарту на Dapper дивіться проект AdoNet-ThreeLayer-Ex-02.
    /// </remarks>
    // [ApiController] вмикає автоматичну валідацію моделі, зв'язування параметрів
    // та коректні відповіді 400 при некоректному тілі запиту.
    [ApiController]
    // Базовий маршрут контролера: усі ендпойнти починаються з /Students.
    [Route("/Students")]
    public class StudentsController : ControllerBase
    {
        // Сервіс бізнес-логіки; контролер лише приймає HTTP-запити й делегує роботу йому.
        readonly StudentsService _service;

        // Залежність впроваджується через конструктор (DI).
        public StudentsController(StudentsService studentsService)
        {
            _service = studentsService;
        }

        // GET /Students?page=1&count=10 — сторінка списку студентів.
        [HttpGet()]
        public ActionResult<IEnumerable<Student>> GetStudents(int page = 1, int count = 10)
        {
            // Проста перевірка вхідних параметрів пагінації.
            if (page <= 0 || count <= 0)
            {
                return BadRequest("Page or count is less or equal to 0"); // 400
            }

            return Ok(_service.GetStudents(page, count)); // 200 + JSON-масив
        }

        // GET /Students/Count — загальна кількість студентів.
        [HttpGet("Count")]
        public ActionResult<int> GetStudentsCount()
        {
            return Ok(_service.StudentsCount());
        }

        // GET /Students/{id} — один студент за ідентифікатором.
        [HttpGet("{id}")]
        public ActionResult<Student> GetStudent(int id)
        {
            var student = _service.GetStudent(id);

            if (student == null)
            {
                return NotFound(); // 404, якщо студента немає
            }

            return Ok(student); // 200 + JSON-об'єкт
        }

        // POST /Students/AddStudent — створення нового студента; дані беруться з тіла запиту (JSON).
        [HttpPost(nameof(AddStudent))]
        public IActionResult AddStudent([FromBody] Student student)
        {
            // Мінімальна валідація обов'язкових полів.
            if (string.IsNullOrWhiteSpace(student.Email))
            {
                return BadRequest("Email is required");
            }

            if (string.IsNullOrWhiteSpace(student.Name))
            {
                return BadRequest("Name is required");
            }

            // Перетворюємо вхідний record на модель предметної області й передаємо сервісу.
            _service.AddStudent(new Models.Student { Email = student.Email, Name = student.Name });
            return StatusCode((int)System.Net.HttpStatusCode.Created); // 201
        }

        // DELETE /Students/DeleteStudents/{id} — видалення студента за Id.
        [HttpDelete("DeleteStudents/{id}")]
        public IActionResult DeleteStudents(int id)
        {
            // Сервіс повертає кількість видалених рядків: 0 означає, що студента не знайдено.
            var result = _service.DeleteStudent(id);
            if (result == 0)
            {
                return NotFound();
            }

            return Ok("Deleted Successfully");
        }
    }

    // DTO для вхідних даних POST-запиту (окремий від Models.Student, без поля Id).
    public record class Student(string Name, string Email);
}
