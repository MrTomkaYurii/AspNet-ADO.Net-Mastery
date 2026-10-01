using System.Data.SqlClient;

namespace AdoNet_Explanation_Ex01.Infrastructure;

/// <summary>
/// Спільна "інфраструктура" для прикладів:
/// рядки підключення та підготовка демонстраційної бази даних і таблиці Books.
/// </summary>
/// <remarks>
/// <b>Архітектурна роль інфраструктурного помічника:</b>
/// Демонструє правильне використання <see cref="SqlConnectionStringBuilder"/> для безпечного
/// маніпулювання параметрами підключення та ізольованого створення бази даних без хардкоду.
/// </remarks>
internal static class Db
{
    /// <summary>Назва демонстраційної бази даних.</summary>
    public const string DatabaseName = "AdoNet_Explanation_Ex01";

    /// <summary>Рядок підключення до сервера (системна база master).</summary>
    public static string ServerConnectionString => AppConfig.ServerConnectionString;

    /// <summary>
    /// Рядок підключення саме до нашої бази. Будуємо його з серверного рядка,
    /// підставляючи назву бази через SqlConnectionStringBuilder (щоб не збирати рядок вручну).
    /// </summary>
    public static string DatabaseConnectionString => new SqlConnectionStringBuilder(ServerConnectionString)
    {
        InitialCatalog = DatabaseName
    }.ConnectionString;

    /// <summary>
    /// Створює базу та таблицю Books, якщо їх ще немає.
    /// Це той самий код, що й у прикладі "Створення бази й таблиці", але з перевірками IF —
    /// щоб інші приклади могли викликати його багато разів поспіль без помилок.
    /// </summary>
    public static async Task EnsureCreatedAsync()
    {
        // Крок 1. Підключаємось до сервера й створюємо базу, якщо її ще немає.
        using (SqlConnection serverConnection = new(ServerConnectionString))
        {
            await serverConnection.OpenAsync();

            using SqlCommand createDatabase = serverConnection.CreateCommand();
            createDatabase.CommandText =
                $"IF DB_ID(N'{DatabaseName}') IS NULL CREATE DATABASE [{DatabaseName}];";
            await createDatabase.ExecuteNonQueryAsync();
        }

        // Крок 2. Підключаємось уже до самої бази й створюємо таблицю, якщо її ще немає.
        using SqlConnection dbConnection = new(DatabaseConnectionString);
        await dbConnection.OpenAsync();

        using SqlCommand createTable = dbConnection.CreateCommand();
        createTable.CommandText = @"
            IF OBJECT_ID(N'dbo.Books', N'U') IS NULL
            CREATE TABLE Books
            (
                Id        INT           IDENTITY(1,1) PRIMARY KEY,
                Title     NVARCHAR(100) NOT NULL,
                Author    NVARCHAR(100) NOT NULL,
                PageCount INT           NOT NULL
            );";
        await createTable.ExecuteNonQueryAsync();
    }

    /// <summary>
    /// Приводить таблицю Books до відомого стану: очищає її та вставляє 4 книги.
    /// Приклади, для яких запис у базу — не головна тема, викликають цей метод,
    /// щоб щоразу починати з однакових даних.
    /// </summary>
    public static async Task SeedAsync()
    {
        await EnsureCreatedAsync();

        using SqlConnection connection = new(DatabaseConnectionString);
        await connection.OpenAsync();

        using SqlCommand command = connection.CreateCommand();
        // TRUNCATE очищає таблицю й скидає лічильник IDENTITY, тому Id завжди починаються з 1.
        command.CommandText = @"
            TRUNCATE TABLE Books;
            INSERT INTO Books (Title, Author, PageCount) VALUES
                (N'To Kill a Mockingbird', N'Harper Lee',          281),
                (N'1984',                   N'George Orwell',       328),
                (N'The Great Gatsby',       N'F. Scott Fitzgerald', 180),
                (N'Brave New World',        N'Aldous Huxley',       311);";
        await command.ExecuteNonQueryAsync();
    }

    /// <summary>Видаляє демонстраційну базу даних разом з усіма даними.</summary>
    public static async Task DropDatabaseAsync()
    {
        using SqlConnection serverConnection = new(ServerConnectionString);
        await serverConnection.OpenAsync();

        using SqlCommand dropDatabase = serverConnection.CreateCommand();
        // SET SINGLE_USER WITH ROLLBACK IMMEDIATE закриває всі інші підключення до бази,
        // інакше DROP DATABASE впаде з помилкою "database is currently in use".
        dropDatabase.CommandText = $@"
            IF DB_ID(N'{DatabaseName}') IS NOT NULL
            BEGIN
                ALTER DATABASE [{DatabaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                DROP DATABASE [{DatabaseName}];
            END";
        await dropDatabase.ExecuteNonQueryAsync();
    }

    /// <summary>
    /// Друкує вміст таблиці Books у вигляді таблиці.
    /// Використовується в прикладах, де читання даних — не головна тема
    /// (наприклад, щоб показати результат після INSERT/UPDATE чи транзакції).
    /// </summary>
    public static async Task PrintBooksAsync()
    {
        using SqlConnection connection = new(DatabaseConnectionString);
        await connection.OpenAsync();

        using SqlCommand select = new("SELECT Id, Title, Author, PageCount FROM Books ORDER BY Id;", connection);
        using SqlDataReader reader = await select.ExecuteReaderAsync();

        Console.WriteLine($"{"Id",-4} {"Назва",-26} {"Автор",-22} {"Стор.",6}");
        Console.WriteLine(new string('-', 62));

        while (await reader.ReadAsync())
        {
            Console.WriteLine($"{reader.GetInt32(0),-4} {reader.GetString(1),-26} {reader.GetString(2),-22} {reader.GetInt32(3),6}");
        }
    }
}
