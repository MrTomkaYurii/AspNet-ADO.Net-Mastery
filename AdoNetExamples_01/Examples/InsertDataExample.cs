using System.Data.SqlClient;
using AdoNet_Explanation_Ex01.Infrastructure;

namespace AdoNet_Explanation_Ex01.Examples;

/// <summary>
/// Приклад 3. Додавання рядків (INSERT).
/// Найпростіший запис у базу: один SqlCommand з текстом INSERT і ExecuteNonQueryAsync,
/// який повертає кількість доданих рядків.
/// </summary>
internal sealed class InsertDataExample : IExample
{
    public string Title => "Додавання рядків (INSERT, ExecuteNonQuery)";

    public async Task RunAsync()
    {
        await Db.EnsureCreatedAsync();

        using SqlConnection connection = new(Db.DatabaseConnectionString);
        await connection.OpenAsync();

        // Очищаємо таблицю, щоб приклад щоразу починався з порожньої.
        // TRUNCATE ще й скидає лічильник IDENTITY, тому Id знову підуть від 1.
        using (SqlCommand clear = new("TRUNCATE TABLE Books;", connection))
        {
            await clear.ExecuteNonQueryAsync();
        }

        // Один INSERT може додати одразу кілька рядків.
        const string sql = @"
            INSERT INTO Books (Title, Author, PageCount) VALUES
                (N'To Kill a Mockingbird', N'Harper Lee',          281),
                (N'1984',                   N'George Orwell',       328),
                (N'The Great Gatsby',       N'F. Scott Fitzgerald', 180),
                (N'Brave New World',        N'Aldous Huxley',       311);";

        using SqlCommand insert = new(sql, connection);

        // ExecuteNonQueryAsync повертає кількість рядків, які зачепила команда.
        int rowsAdded = await insert.ExecuteNonQueryAsync();
        Console.WriteLine($"Додано рядків: {rowsAdded}");

        Console.WriteLine();
        await Db.PrintBooksAsync();
    }
}
