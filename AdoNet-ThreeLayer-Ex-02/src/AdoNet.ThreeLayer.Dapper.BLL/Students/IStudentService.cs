namespace AdoNet.ThreeLayer.Dapper.BLL.Students;

/// <summary>
/// Контракт бізнес-логіки для роботи зі студентами.
/// Шар API викликає саме ці методи й не знає нічого про SQL чи Dapper.
/// </summary>
public interface IStudentService
{
    Task<IReadOnlyList<StudentDto>> GetStudentsAsync(int page, int count, CancellationToken ct = default);

    Task<int> GetStudentsCountAsync(CancellationToken ct = default);

    Task<StudentDto?> GetStudentAsync(int id, CancellationToken ct = default);

    /// <summary>Перевіряє дані й додає студента. Кидає <see cref="ValidationException"/> за некоректних даних.</summary>
    Task AddStudentAsync(CreateStudentDto student, CancellationToken ct = default);

    /// <summary>Видаляє студента. Повертає false, якщо студента з таким Id не було.</summary>
    Task<bool> DeleteStudentAsync(int id, CancellationToken ct = default);
}
