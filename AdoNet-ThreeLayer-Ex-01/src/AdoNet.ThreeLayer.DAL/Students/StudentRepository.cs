using Microsoft.Data.SqlClient;

namespace AdoNet.ThreeLayer.DAL.Students;

/// <summary>
/// Реалізація <see cref="IStudentRepository"/> на "чистому" ADO.NET.
/// Тут і тільки тут програма знає про SqlConnection, SqlCommand, SqlDataReader і текст SQL.
/// Запити взяті з базового прикладу AdoNet-WebApi-Ex-01.
/// </summary>
/// <remarks>
/// <b>Архітектурні особливості низькорівневої реалізації:</b>
/// 1. <b>Ручний мапінг (Manual Hydration):</b>
///    Кожен стовпець читається вручну через <see cref="SqlDataReader.GetOrdinal"/> та типізовані
///    методи <c>GetInt32</c>, <c>GetString</c>. Це еталон швидкості читання, але вимагає багато
///    шаблонного коду, схильного до помилок при зміні схеми.
/// 2. <b>Керування ресурсами:</b>
///    Всі ресурси (<see cref="SqlConnection"/>, <see cref="SqlCommand"/>, <see cref="SqlDataReader"/>)
///    огортаються у <c>await using</c>, гарантуючи своєчасний виклик <c>DisposeAsync</c> і повернення
///    з'єднання в пул навіть у разі виникнення винятку в базі.
/// 3. <b>Параметризація:</b>
///    Використовується колекція <c>command.Parameters</c>, що унеможливлює ін'єкцію SQL.
/// </remarks>
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
        await connection.OpenAsync(ct);

        await using SqlCommand command = new(sql, connection);
        command.Parameters.AddWithValue("@Offset", offset);
        command.Parameters.AddWithValue("@PageSize", pageSize);

        var students = new List<Student>();
        await using SqlDataReader reader = await command.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            students.Add(Map(reader));
        }

        return students;
    }

    public async Task<int> CountAsync(CancellationToken ct = default)
    {
        const string sql = "SELECT COUNT(*) FROM Students";

        await using SqlConnection connection = new(_connectionString);
        await connection.OpenAsync(ct);

        await using SqlCommand command = new(sql, connection);

        // ExecuteScalar повертає перший стовпець першого рядка — тут це число з COUNT(*).
        object? result = await command.ExecuteScalarAsync(ct);
        return Convert.ToInt32(result);
    }

    public async Task<Student?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        const string sql = "SELECT * FROM Students WHERE Id = @Id";

        await using SqlConnection connection = new(_connectionString);
        await connection.OpenAsync(ct);

        await using SqlCommand command = new(sql, connection);
        command.Parameters.AddWithValue("@Id", id);

        await using SqlDataReader reader = await command.ExecuteReaderAsync(ct);
        return await reader.ReadAsync(ct) ? Map(reader) : null;
    }

    public async Task<int> InsertAsync(Student student, CancellationToken ct = default)
    {
        const string sql = "INSERT INTO Students (Name, Email) VALUES (@Name, @Email)";

        await using SqlConnection connection = new(_connectionString);
        await connection.OpenAsync(ct);

        await using SqlCommand command = new(sql, connection);
        command.Parameters.AddWithValue("@Name", student.Name);
        command.Parameters.AddWithValue("@Email", student.Email);

        // ExecuteNonQuery використовують для INSERT/UPDATE/DELETE — повертає кількість зачеплених рядків.
        return await command.ExecuteNonQueryAsync(ct);
    }

    public async Task<int> DeleteAsync(int id, CancellationToken ct = default)
    {
        const string sql = "DELETE FROM Students WHERE Id = @Id";

        await using SqlConnection connection = new(_connectionString);
        await connection.OpenAsync(ct);

        await using SqlCommand command = new(sql, connection);
        command.Parameters.AddWithValue("@Id", id);

        return await command.ExecuteNonQueryAsync(ct);
    }

    /// <summary>Перетворює поточний рядок SqlDataReader на об'єкт <see cref="Student"/>.</summary>
    private static Student Map(SqlDataReader reader) => new()
    {
        Id = reader.GetInt32(reader.GetOrdinal("Id")),
        Name = reader.GetString(reader.GetOrdinal("Name")),
        Email = reader.GetString(reader.GetOrdinal("Email")),
    };
}
