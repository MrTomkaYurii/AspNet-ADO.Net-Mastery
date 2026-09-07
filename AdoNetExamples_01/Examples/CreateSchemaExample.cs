using System.Data.SqlClient;
using AdoNet_Explanation_Ex01.Infrastructure;

namespace AdoNet_Explanation_Ex01.Examples;

/// <summary>
/// Приклад 2. Створення бази даних і таблиці (DDL).
/// Команди CREATE DATABASE / CREATE TABLE не повертають рядків, тому їх виконують
/// методом ExecuteNonQueryAsync — "виконай і не чекай таблицю результатів".
/// </summary>
internal sealed class CreateSchemaExample : IExample
{
    public string Title => "Створення бази даних і таблиці (CREATE DATABASE / TABLE)";

    public async Task RunAsync()
    {
        // Починаємо з чистого аркуша, щоб побачити "голі" команди CREATE без перевірок IF.
        await Db.DropDatabaseAsync();

        // --- CREATE DATABASE ---
        // Щоб створити базу, підключаємось до сервера (до системної бази master).
        using (SqlConnection serverConnection = new(Db.ServerConnectionString))
        {
            await serverConnection.OpenAsync();

            using SqlCommand command = new($"CREATE DATABASE [{Db.DatabaseName}];", serverConnection);
            await command.ExecuteNonQueryAsync();
            Console.WriteLine($"CREATE DATABASE [{Db.DatabaseName}] — виконано.");
        }

        // --- CREATE TABLE ---
        // Тепер підключаємось уже до створеної бази (її назва задана в DatabaseConnectionString).
        using (SqlConnection dbConnection = new(Db.DatabaseConnectionString))
        {
            await dbConnection.OpenAsync();

            using SqlCommand command = new(@"
                CREATE TABLE Books
                (
                    Id        INT           IDENTITY(1,1) PRIMARY KEY,  -- лічильник, заповнюється автоматично
                    Title     NVARCHAR(100) NOT NULL,
                    Author    NVARCHAR(100) NOT NULL,
                    PageCount INT           NOT NULL
                );", dbConnection);
            await command.ExecuteNonQueryAsync();
            Console.WriteLine("CREATE TABLE Books — виконано.");
        }

        Console.WriteLine();
        Console.WriteLine("Порожня таблиця Books готова. Далі скористайтесь прикладом 3, щоб додати дані.");
    }
}
