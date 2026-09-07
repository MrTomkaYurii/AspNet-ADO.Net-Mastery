using System.Data;
using System.Data.SqlClient;
using AdoNet_Explanation_Ex01.Infrastructure;

namespace AdoNet_Explanation_Ex01.Examples;

/// <summary>
/// Приклад 9. Відключена модель: SqlDataAdapter + DataTable.
/// Адаптер завантажує результат запиту в DataTable у пам'яті й одразу закриває
/// з'єднання. Дані можна змінювати офлайн, а потім одним викликом Update
/// надіслати всі зміни (INSERT/UPDATE/DELETE) назад у базу.
/// </summary>
internal sealed class DataAdapterExample : IExample
{
    public string Title => "Відключена модель (SqlDataAdapter + DataTable)";

    public async Task RunAsync()
    {
        await Db.SeedAsync();

        using SqlConnection connection = new(Db.DatabaseConnectionString);
        using SqlDataAdapter adapter = new(
            "SELECT Id, Title, Author, PageCount FROM Books ORDER BY Id;", connection);

        // SqlCommandBuilder сам згенерує команди INSERT/UPDATE/DELETE для adapter.Update
        // (працює, бо в SELECT є первинний ключ Id з однієї таблиці).
        using SqlCommandBuilder builder = new(adapter);

        // Fill сам відкриває з'єднання, читає всі рядки в DataTable і закриває з'єднання.
        DataTable books = new();
        adapter.Fill(books);
        Console.WriteLine($"Завантажено в DataTable: {books.Rows.Count} рядків (з'єднання вже закрите).");

        // --- Зміни лише в пам'яті, без звернень до бази ---
        books.Rows[0]["PageCount"] = (int)books.Rows[0]["PageCount"] + 1;  // змінили рядок

        DataRow newRow = books.NewRow();                                   // додали рядок
        newRow["Title"] = "Animal Farm";
        newRow["Author"] = "George Orwell";
        newRow["PageCount"] = 112;
        books.Rows.Add(newRow);

        books.Rows[1].Delete();                                            // позначили рядок на видалення

        // --- Один виклик надсилає всі накопичені зміни в базу ---
        int affected = adapter.Update(books);
        Console.WriteLine($"adapter.Update надіслав змін: {affected} (1 зміна + 1 додавання + 1 видалення).");

        Console.WriteLine();
        await Db.PrintBooksAsync();
    }
}
