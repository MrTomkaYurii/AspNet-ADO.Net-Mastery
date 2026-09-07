using System.Data.SqlClient;
using AdoNet_Explanation_Ex01.Infrastructure;

namespace AdoNet_Explanation_Ex01.Examples;

/// <summary>
/// Приклад 5. Читання даних курсором (SqlDataReader).
/// SqlDataReader читає рядки по одному й лише вперед. Це найшвидший і найощадніший
/// за пам'яттю спосіб дістати дані: у пам'яті тримається тільки поточний рядок.
/// </summary>
internal sealed class DataReaderExample : IExample
{
    public string Title => "Читання даних курсором (SqlDataReader)";

    public async Task RunAsync()
    {
        await Db.SeedAsync();

        using SqlConnection connection = new(Db.DatabaseConnectionString);
        await connection.OpenAsync();

        using SqlCommand command = new(
            "SELECT Id, Title, Author, PageCount FROM Books ORDER BY Id;", connection);

        // ExecuteReaderAsync повертає SqlDataReader, наведений на початок результату.
        using SqlDataReader reader = await command.ExecuteReaderAsync();

        Console.WriteLine($"Стовпців у результаті: {reader.FieldCount}");
        Console.WriteLine();

        // ReadAsync переходить до наступного рядка й повертає false, коли рядки скінчились.
        while (await reader.ReadAsync())
        {
            // Значення дістаємо за індексом стовпця. Типізовані методи одразу
            // повертають потрібний тип (без ручного приведення з object).
            int id = reader.GetInt32(0);
            string title = reader.GetString(1);
            string author = reader.GetString(2);
            int pageCount = reader.GetInt32(3);

            // Те саме можна робити за іменем стовпця: (string)reader["Title"]
            Console.WriteLine($"#{id}: \"{title}\" — {author}, {pageCount} стор.");
        }
    }
}
