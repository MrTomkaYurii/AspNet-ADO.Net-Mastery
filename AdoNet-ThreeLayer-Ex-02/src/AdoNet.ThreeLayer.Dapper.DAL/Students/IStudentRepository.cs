namespace AdoNet.ThreeLayer.Dapper.DAL.Students;

/// <summary>
/// Контракт доступу до даних студентів.
/// BLL працює саме з цим інтерфейсом, а не з конкретною реалізацією на Dapper —
/// тому шар бізнес-логіки не залежить від того, як саме зберігаються дані.
/// Сигнатури методів навмисно ідентичні AdoNet-ThreeLayer-Ex-01, щоб було видно:
/// міняється лише реалізація (DAL), контракт і решта шарів — ні.
/// </summary>
/// <remarks>
/// <b>Архітектурна роль (DIP / Repository Pattern):</b>
/// Інтерфейс реалізує принцип інверсії залежностей (SOLID Dependency Inversion Principle).
/// Шар бізнес-логіки (BLL) залежить від абстракції <see cref="IStudentRepository"/>,
/// а не від бібліотеки Dapper або конкретної СУБД. Це дозволяє безболісно замінювати
/// рівень збереження (Dapper, чистий ADO.NET, EF Core або InMemory Mock для unit-тестів)
/// без внесення змін до бізнес-правил.
/// 
/// Усі методи підтримують асинхронність із передачею <see cref="CancellationToken"/>
/// для своєчасного переривання SQL-запитів і запобігання блокуванню пулу з'єднань.
/// </remarks>
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
