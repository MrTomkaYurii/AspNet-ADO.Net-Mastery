using System.Data.SqlClient;
using AdoNet_Explanation_Ex01.Infrastructure;

namespace AdoNet_Explanation_Ex01.Examples;

/// <summary>
/// Приклад 6. Одне-єдине значення (ExecuteScalar).
/// Коли запит повертає лише один рядок і один стовпець (COUNT, MIN, AVG...),
/// ExecuteScalarAsync віддає це значення напряму — без SqlDataReader.
/// </summary>
internal sealed class ScalarQueryExample : IExample
{
    public string Title => "Одне значення із запиту (ExecuteScalar)";

    public async Task RunAsync()
    {
        await Db.SeedAsync();

        using SqlConnection connection = new(Db.DatabaseConnectionString);
        await connection.OpenAsync();

        // Один об'єкт SqlCommand можна перевикористати, щоразу міняючи CommandText.
        using SqlCommand command = connection.CreateCommand();

        command.CommandText = "SELECT COUNT(*) FROM Books;";
        // Результат має тип object — приводимо його до потрібного типу.
        int count = Convert.ToInt32(await command.ExecuteScalarAsync());

        command.CommandText = "SELECT MIN(PageCount) FROM Books;";
        int minPages = Convert.ToInt32(await command.ExecuteScalarAsync());

        command.CommandText = "SELECT AVG(PageCount) FROM Books;";
        int avgPages = Convert.ToInt32(await command.ExecuteScalarAsync());

        Console.WriteLine($"Усього книг:            {count}");
        Console.WriteLine($"Найменше сторінок:      {minPages}");
        Console.WriteLine($"Середня к-ть сторінок:  {avgPages}");
    }
}
