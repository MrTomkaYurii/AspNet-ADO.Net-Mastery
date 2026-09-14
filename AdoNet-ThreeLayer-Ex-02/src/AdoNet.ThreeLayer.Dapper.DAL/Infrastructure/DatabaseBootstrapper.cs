using Dapper;
using Microsoft.Data.SqlClient;

namespace AdoNet.ThreeLayer.Dapper.DAL.Infrastructure;

/// <summary>
/// Одноразова підготовка бази для прикладу: створює базу, таблицю Students
/// і, якщо таблиця порожня, додає кілька тестових рядків.
/// Викликається під час старту API, щоб приклад запускався без ручного налаштування.
/// Той самий скрипт лежить у файлі db/schema.sql.
/// </summary>
public sealed class DatabaseBootstrapper
{
    private readonly string _databaseConnectionString;

    public DatabaseBootstrapper(string databaseConnectionString)
    {
        _databaseConnectionString = databaseConnectionString;
    }

    public async Task EnsureCreatedAsync(CancellationToken ct = default)
    {
        // З рядка підключення дістаємо назву бази й будуємо окремий рядок до системної бази master.
        var builder = new SqlConnectionStringBuilder(_databaseConnectionString);
        string databaseName = builder.InitialCatalog;

        string serverConnectionString = new SqlConnectionStringBuilder(_databaseConnectionString)
        {
            InitialCatalog = "master"
        }.ConnectionString;

        // 1. Створюємо базу (підключення до master).
        // connection.ExecuteAsync(...) із Dapper — те саме, що SqlCommand.ExecuteNonQueryAsync
        // у "чистому" ADO.NET, тільки без ручного new SqlCommand(sql, connection).
        await using (SqlConnection master = new(serverConnectionString))
        {
            await master.ExecuteAsync(new CommandDefinition(
                $"IF DB_ID(N'{databaseName}') IS NULL CREATE DATABASE [{databaseName}];",
                cancellationToken: ct));
        }

        // 2. Створюємо таблицю та за потреби наповнюємо її (підключення вже до нашої бази).
        // Явний Open() не потрібен — Dapper сам відкриває закрите з'єднання перед виконанням.
        await using SqlConnection db = new(_databaseConnectionString);

        const string createTableAndSeed = @"
            IF OBJECT_ID(N'dbo.Students', N'U') IS NULL
            CREATE TABLE dbo.Students
            (
                Id    INT           IDENTITY(1,1) CONSTRAINT PK_Students PRIMARY KEY,
                Name  NVARCHAR(255) NOT NULL,
                Email NVARCHAR(255) NOT NULL
            );

            IF NOT EXISTS (SELECT 1 FROM dbo.Students)
            INSERT INTO dbo.Students (Name, Email) VALUES
                (N'John Doe',        N'john.doe@email.com'),
                (N'Jane Smith',      N'jane.smith@email.com'),
                (N'Bob Johnson',     N'bob.johnson@email.com'),
                (N'Alice Williams',  N'alice.williams@email.com'),
                (N'Tom Brown',       N'tom.brown@email.com');";

        await db.ExecuteAsync(new CommandDefinition(createTableAndSeed, cancellationToken: ct));
    }
}
