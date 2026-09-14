using LearnAdoDotnet.Models;
using Microsoft.Data.SqlClient;

/// <summary>
/// Сервіс бізнес-логіки для роботи зі студентами.
/// Формує SQL-запити з параметрами та делегує їх виконання репозиторію DataAccessRepository.
/// Контролер працює саме з цим класом, не знаючи деталей ADO.NET.
/// </summary>
public class StudentsService
{
    // Репозиторій доступу до даних, впроваджується через DI (див. Program.cs).
    private readonly DataAccessRepository _dataAccessService;

    public StudentsService(DataAccessRepository dataAccessService)
    {
        _dataAccessService = dataAccessService;
    }

    // TODO: Separate query strings into different class
    // TODO: винести рядки запитів в окремий клас (щоб не змішувати SQL і логіку).

    /// <summary>Додає нового студента в таблицю Students.</summary>
    public void AddStudent(Student student)
    {
        // Значення передаються через параметри @Name / @Email, а не конкатенацією — захист від SQL-ін'єкцій.
        var query = "INSERT INTO Students (Name, Email) VALUES (@Name, @Email)";
        var parameters = new SqlParameter[]
        {
            new SqlParameter("@Name", student.Name),
            new SqlParameter("@Email", student.Email)
        };

        // INSERT не повертає рядків, тому використовуємо ExecuteNonQuery.
        _dataAccessService.ExecuteNonQuery(query, parameters);
    }

    /// <summary>Повертає одного студента за Id або null, якщо його не знайдено.</summary>
    public Student? GetStudent(int id)
    {
        var query = "SELECT * FROM Students WHERE Id = @Id ";
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@Id", id)
        };

        var dataReader = _dataAccessService.ExecuteReader(query, parameters);

        // Read() зчитує наступний рядок і повертає false, коли рядків більше немає.
        if (dataReader.Read())
        {
            Student student = new Student();
            // Use this
            // Варіант 1 — доступ за індексом стовпця (швидше, але залежить від порядку стовпців):
            // student.Id = dataReader.GetInt32(0);
            // student.Name = dataReader.GetString(1);
            // student.Email = dataReader.GetString(2);
            // Or this
            // Варіант 2 — доступ за назвою стовпця з приведенням типу (наочніше):
            student.Id = (int) dataReader["Id"];
            student.Name = (string) dataReader["Name"];
            student.Email = (string) dataReader["Email"];
            return student;
        }

        return null;
    }

    /// <summary>
    /// Повертає сторінку студентів. Пагінація реалізована засобами T-SQL:
    /// OFFSET ... ROWS FETCH NEXT ... ROWS ONLY (потребує ORDER BY).
    /// </summary>
    public List<Student> GetStudents(int pageNumber = 1, int pageSize = 10)
    {
        // Скільки рядків пропустити: для 1-ї сторінки — 0, для 2-ї — pageSize, і т.д.
        var offset = (pageNumber - 1) * pageSize;
        if (offset < 0 || pageSize <= 0)
        {
            // Некоректні параметри — повертаємо порожній список замість помилки.
            return new List<Student>();
        }

        var query = "SELECT * FROM Students ORDER BY Id OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
        var parameters = new SqlParameter[]
        {
            new SqlParameter("@Offset", offset),
            new SqlParameter("@PageSize", pageSize)
        };

        var dataReader = _dataAccessService.ExecuteReader(query, parameters);
        var students = new List<Student>();

        // Проходимо всі рядки результату по черзі й перетворюємо кожен на об'єкт Student.
        while (dataReader.Read())
        {
            Student student = new Student();
            // Read data from the dataReader
            // Читання за індексом стовпця: 0 = Id, 1 = Name, 2 = Email.
            student.Id = dataReader.GetInt32(0);
            student.Name = dataReader.GetString(1);
            student.Email = dataReader.GetString(2);

            students.Add(student);
        }

        return students;
    }

    /// <summary>Повертає загальну кількість студентів (одне число).</summary>
    public object? StudentsCount()
    {
        var query = "SELECT COUNT(*) FROM Students";

        // COUNT(*) — це одне значення, тому використовуємо ExecuteScalar.
        return _dataAccessService.ExecuteScalar(query); ;
    }

    /// <summary>
    /// Видаляє студента за Id. Повертає кількість видалених рядків
    /// (0 — якщо студента з таким Id не було).
    /// </summary>
    public int DeleteStudent(int id)
    {
        var query = "DELETE FROM Students WHERE Id = @Id";
        var parameters = new SqlParameter[]
        {
            new SqlParameter("@Id", id)
        };

        return _dataAccessService.ExecuteNonQuery(query, parameters);
    }
}
