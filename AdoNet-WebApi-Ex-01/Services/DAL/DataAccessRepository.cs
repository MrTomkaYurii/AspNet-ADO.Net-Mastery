using Microsoft.Data.SqlClient;
using System.Data;
using LearnAdoDotnet.Settings;

/// <summary>
/// Репозиторій низькорівневого доступу до даних через "чистий" ADO.NET.
/// Інкапсулює роботу з SqlConnection / SqlCommand і надає три базові операції:
///  - ExecuteNonQuery — команди без результату-набору (INSERT/UPDATE/DELETE);
///  - ExecuteReader   — запити, що повертають рядки (SELECT);
///  - ExecuteScalar   — запити, що повертають одне значення (наприклад COUNT).
/// Реалізує IDisposable, щоб коректно звільняти з'єднання з БД.
/// </summary>
public class DataAccessRepository : IDisposable
{
    // Рядок підключення до SQL Server.
    private readonly string? _connectionString;

    // Одне з'єднання, яке перевикористовується методами цього екземпляра.
    private readonly SqlConnection _sqlConnection;

    // ! this is bad
    // ! Do not hard code the connection string ever
    // ! ПОГАНО: ніколи не "зашивайте" рядок підключення прямо в код —
    // ! його не можна змінити без перекомпіляції, а паролі потрапляють у систему контролю версій.
    // public DataAccessService()
    // {
    //     _connectionString =
    //         "Data Source=MAHMOUD\\SQLEXPRESS;Initial Catalog=AdvancedProgramming2;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
    //     _sqlConnection = new SqlConnection(_connectionString);
    // }

    // Better (preferred)
    // КРАЩЕ: рядок підключення надходить ззовні через DI (з appsettings.json).
    public DataAccessRepository(ConnectionStrings connectionStrings)
    {
        _connectionString = connectionStrings.DefaultConnection;

        // Створення об'єкта SqlConnection саме по собі не відкриває з'єднання —
        // фактичне підключення відбувається під час виклику Open().
        _sqlConnection = new SqlConnection(_connectionString);
    }

    /// <summary>
    /// > Execute a SQL query and return the number of rows affected
    /// </summary>
    /// <param name="query">The SQL query to execute.</param>
    /// <param name="parameters"></param>
    /// <returns>
    /// The number of rows affected by the query.
    /// </returns>
    public int ExecuteNonQuery(string query, IEnumerable<SqlParameter>? parameters = null)
    {
        // SqlCommand об'єднує текст запиту та з'єднання; using гарантує звільнення ресурсів команди.
        using (var command = new SqlCommand(query, _sqlConnection))
        {
            // Додаємо параметри запиту (@Name, @Id ...). Параметри — захист від SQL-ін'єкцій
            // і коректна передача типів; ніколи не склеюйте значення в рядок запиту вручну.
            if (parameters != null && parameters.Any())
            {
                foreach (var param in parameters)
                {
                    command.Parameters.Add(param);
                }
            }

            try
            {
                _sqlConnection.Open();

                // ExecuteNonQuery повертає кількість змінених рядків (для INSERT/UPDATE/DELETE).
                return command.ExecuteNonQuery();
            }
            catch (System.Exception)
            {
                // Спрощена обробка: логуємо факт помилки і "пробрасуємо" виняток далі (throw),
                // зберігаючи оригінальний стек викликів.
                System.Console.WriteLine("Error Details");
                throw;
            }
        }
    }

    /// <summary>
    /// > Execute a query and return the results as a data reader
    /// </summary>
    /// <param name="query">The SQL query to execute</param>
    /// <param name="parameters"></param>
    /// <returns>
    /// A SqlDataReader object.
    /// </returns>
    public IDataReader ExecuteReader(string query, IEnumerable<SqlParameter>? parameters = null)
    {
        using (var command = new SqlCommand(query, _sqlConnection))
        {
            if (parameters != null && parameters.Any())
            {
                foreach (var param in parameters)
                {
                    command.Parameters.Add(param);
                }
            }

            try
            {
                _sqlConnection.Open();

                // ExecuteReader повертає SqlDataReader — потоковий "курсор" по рядках результату.
                // Дані читаються по одному рядку (reader.Read()), поки з'єднання відкрите.
                // Увага: тут reader повертається назовні, тому з'єднання лишається відкритим,
                // доки викликач не завершить читання (і не звільнить reader).
                return command.ExecuteReader();
            }
            catch (System.Exception)
            {
                System.Console.WriteLine("Error Details");
                throw;
            }
        }
    }

    /// <summary>
    /// Виконує запит, який повертає одне скалярне значення (перший стовпець першого рядка),
    /// наприклад SELECT COUNT(*) FROM Students.
    /// </summary>
    public object? ExecuteScalar(string query, IEnumerable<SqlParameter>? parameters = null)
    {
        // Тут навмисно створюється окреме короткоживуче з'єднання — обидва using
        // закривають і команду, і з'єднання одразу після виконання запиту.
        using (var connection = new SqlConnection(_connectionString))
        using (var command = new SqlCommand(query, connection))
        {
            if (parameters != null && parameters.Any())
            {
                foreach (var param in parameters)
                {
                    command.Parameters.Add(param);
                }
            }

            try
            {
                connection.Open();

                // ExecuteScalar повертає object? — результат потрібно привести до потрібного типу.
                return command.ExecuteScalar();
            }
            catch (System.Exception)
            {
                System.Console.WriteLine("Error Details");
                throw;
            }
        }
    }

    /// <summary>
    /// Звільняє з'єднання з БД. Викликається DI-контейнером наприкінці життя об'єкта
    /// (репозиторій зареєстрований як Transient у Program.cs).
    /// </summary>
    public void Dispose()
    {
        _sqlConnection.Dispose();
    }
}
