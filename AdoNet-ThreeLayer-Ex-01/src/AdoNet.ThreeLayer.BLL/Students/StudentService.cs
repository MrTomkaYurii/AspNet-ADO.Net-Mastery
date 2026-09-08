using AdoNet.ThreeLayer.DAL.Students;

namespace AdoNet.ThreeLayer.BLL.Students;

/// <summary>
/// Бізнес-логіка роботи зі студентами.
/// Відповідає за: перевірку вхідних даних, розрахунок зсуву для сторінок,
/// перетворення сутностей DAL (<see cref="Student"/>) у DTO (<see cref="StudentDto"/>).
/// Доступ до бази виконує через <see cref="IStudentRepository"/> — самого ADO.NET тут немає.
/// </summary>
public sealed class StudentService : IStudentService
{
    private readonly IStudentRepository _repository;

    public StudentService(IStudentRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<StudentDto>> GetStudentsAsync(int page, int count, CancellationToken ct = default)
    {
        // Захист від некоректних значень + переклад "номер сторінки" -> "зсув у рядках".
        if (page < 1) page = 1;
        if (count < 1) count = 10;

        int offset = (page - 1) * count;

        var students = await _repository.GetPageAsync(offset, count, ct);
        return students.Select(ToDto).ToList();
    }

    public Task<int> GetStudentsCountAsync(CancellationToken ct = default) =>
        _repository.CountAsync(ct);

    public async Task<StudentDto?> GetStudentAsync(int id, CancellationToken ct = default)
    {
        var student = await _repository.GetByIdAsync(id, ct);
        return student is null ? null : ToDto(student);
    }

    public async Task AddStudentAsync(CreateStudentDto student, CancellationToken ct = default)
    {
        // Бізнес-правила: ім'я та e-mail обов'язкові.
        if (string.IsNullOrWhiteSpace(student.Name))
            throw new ValidationException("Ім'я студента обов'язкове.");

        if (string.IsNullOrWhiteSpace(student.Email))
            throw new ValidationException("E-mail студента обов'язковий.");

        if (!student.Email.Contains('@'))
            throw new ValidationException("E-mail має некоректний формат.");

        await _repository.InsertAsync(
            new Student { Name = student.Name.Trim(), Email = student.Email.Trim() }, ct);
    }

    public async Task<bool> DeleteStudentAsync(int id, CancellationToken ct = default)
    {
        int deletedRows = await _repository.DeleteAsync(id, ct);
        return deletedRows > 0;
    }

    /// <summary>Перетворення сутності рівня даних на DTO.</summary>
    private static StudentDto ToDto(Student student) =>
        new(student.Id, student.Name, student.Email);
}
