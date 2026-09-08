namespace AdoNet.ThreeLayer.DAL.Students;

/// <summary>
/// Контракт доступу до даних студентів.
/// BLL працює саме з цим інтерфейсом, а не з конкретною реалізацією на ADO.NET —
/// тому шар бізнес-логіки не залежить від того, як саме зберігаються дані.
/// </summary>
public interface IStudentRepository
{
    /// <summary>Повертає сторінку студентів (SELECT ... OFFSET/FETCH).</summary>
    Task<IReadOnlyList<Student>> GetPageAsync(int offset, int pageSize, CancellationToken ct = default);

    /// <summary>Повертає загальну кількість студентів (ExecuteScalar над SELECT COUNT(*)).</summary>
    Task<int> CountAsync(CancellationToken ct = default);

    /// <summary>Повертає студента за Id або null, якщо такого немає.</summary>
    Task<Student?> GetByIdAsync(int id, CancellationToken ct = default);

    /// <summary>Додає студента. Повертає кількість доданих рядків.</summary>
    Task<int> InsertAsync(Student student, CancellationToken ct = default);

    /// <summary>Видаляє студента за Id. Повертає кількість видалених рядків (0, якщо не знайдено).</summary>
    Task<int> DeleteAsync(int id, CancellationToken ct = default);
}
