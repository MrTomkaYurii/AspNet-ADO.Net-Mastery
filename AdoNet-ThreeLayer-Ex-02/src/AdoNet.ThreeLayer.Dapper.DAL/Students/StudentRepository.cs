using Dapper;
using Microsoft.Data.SqlClient;

namespace AdoNet.ThreeLayer.Dapper.DAL.Students;

/// <summary>
/// Реалізація <see cref="IStudentRepository"/> через Dapper поверх Microsoft.Data.SqlClient.
///
/// Dapper — це мікро-ORM: набір extension-методів для IDbConnection
/// (Query/QueryFirstOrDefault/Execute/ExecuteScalar), які самі створюють SqlCommand,
/// додають параметри та мапують результат у об'єкти. Це той самий ADO.NET —
/// SqlConnection нікуди не подівся, Dapper лише прибирає рутину навколо нього.
///
/// Порівняйте з AdoNet-ThreeLayer-Ex-01/src/AdoNet.ThreeLayer.DAL/Students/StudentRepository.cs:
/// SQL той самий, але там — ручний SqlCommand + SqlDataReader + метод Map(),
/// тут — один виклик Dapper-методу з анонімним об'єктом параметрів.
/// </summary>
public sealed class StudentRepository : IStudentRepository
{
    private readonly string _connectionString;

    public StudentRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IReadOnlyList<Student>> GetPageAsync(int offset, int pageSize, CancellationToken ct = default)
    {
        const string sql =
            "SELECT * FROM Students ORDER BY Id OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        await using SqlConnection connection = new(_connectionString);

        // QueryAsync<T> виконує запит і повертає вже готову послідовність об'єктів Student —
        // без циклу while (reader.Read()) і без ручного зчитування стовпців.
        // Другий аргумент — анонімний об'єкт: Dapper сам перетворює його властивості
        // на параметри команди (@Offset, @PageSize), як і SqlParameter у "чистому" ADO.NET.
        var command = new CommandDefinition(sql, new { Offset = offset, PageSize = pageSize }, cancellationToken: ct);
        var students = await connection.QueryAsync<Student>(command);

        return students.AsList();
    }

    public async Task<int> CountAsync(CancellationToken ct = default)
    {
        const string sql = "SELECT COUNT(*) FROM Students";

        await using SqlConnection connection = new(_connectionString);

        // ExecuteScalarAsync<int> одразу повертає значення потрібного типу —
        // без object? і ручного Convert.ToInt32(result), як у "чистому" ADO.NET.
        return await connection.ExecuteScalarAsync<int>(new CommandDefinition(sql, cancellationToken: ct));
    }

    public async Task<Student?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        const string sql = "SELECT * FROM Students WHERE Id = @Id";

        await using SqlConnection connection = new(_connectionString);

        // QueryFirstOrDefaultAsync<T> -> null, якщо рядків немає.
        // Аналог "reader.Read() ? Map(reader) : null" у чистому ADO.NET, але в один рядок.
        var command = new CommandDefinition(sql, new { Id = id }, cancellationToken: ct);
        return await connection.QueryFirstOrDefaultAsync<Student>(command);
    }

    public async Task<int> InsertAsync(Student student, CancellationToken ct = default)
    {
        const string sql = "INSERT INTO Students (Name, Email) VALUES (@Name, @Email)";

        await using SqlConnection connection = new(_connectionString);

        // ExecuteAsync — аналог ExecuteNonQueryAsync: повертає кількість зачеплених рядків.
        // Параметри @Name/@Email Dapper бере прямо з властивостей student через рефлексію.
        var command = new CommandDefinition(sql, new { student.Name, student.Email }, cancellationToken: ct);
        return await connection.ExecuteAsync(command);
    }

    public async Task<int> DeleteAsync(int id, CancellationToken ct = default)
    {
        const string sql = "DELETE FROM Students WHERE Id = @Id";

        await using SqlConnection connection = new(_connectionString);

        var command = new CommandDefinition(sql, new { Id = id }, cancellationToken: ct);
        return await connection.ExecuteAsync(command);
    }

    // Зверніть увагу: тут немає методу Map(SqlDataReader) — Dapper мапує рядки результату
    // у Student сам, зіставляючи назви стовпців (Id, Name, Email) з назвами властивостей.
}
