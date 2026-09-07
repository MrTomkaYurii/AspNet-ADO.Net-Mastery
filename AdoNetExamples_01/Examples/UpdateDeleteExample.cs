using System.Data.SqlClient;
using AdoNet_Explanation_Ex01.Infrastructure;

namespace AdoNet_Explanation_Ex01.Examples;

/// <summary>
/// Приклад 4. Зміна та видалення рядків (UPDATE, DELETE).
/// ExecuteNonQueryAsync повертає, скільки рядків зачепила команда — так ми
/// дізнаємось, чи справді щось змінилось у базі.
/// </summary>
internal sealed class UpdateDeleteExample : IExample
{
    public string Title => "Зміна та видалення рядків (UPDATE, DELETE)";

    public async Task RunAsync()
    {
        // Приводимо таблицю до відомого стану (4 книги).
        await Db.SeedAsync();

        using SqlConnection connection = new(Db.DatabaseConnectionString);
        await connection.OpenAsync();

        // --- UPDATE ---
        using (SqlCommand update = new(
            "UPDATE Books SET PageCount = 250 WHERE Author = N'George Orwell';", connection))
        {
            int updated = await update.ExecuteNonQueryAsync();
            Console.WriteLine($"UPDATE: змінено рядків — {updated}");
        }

        // --- DELETE ---
        using (SqlCommand delete = new(
            "DELETE FROM Books WHERE PageCount < 200;", connection))
        {
            int deleted = await delete.ExecuteNonQueryAsync();
            Console.WriteLine($"DELETE: видалено рядків — {deleted}");
        }

        Console.WriteLine();
        await Db.PrintBooksAsync();
    }
}
