using System.Data;
using System.Data.SqlClient;
using AdoNet_Explanation_Ex01.Infrastructure;

namespace AdoNet_Explanation_Ex01.Examples;

/// <summary>
/// Приклад 7. Параметри запиту (SqlParameter).
/// Значення НІКОЛИ не склеюють із текстом SQL через рядки — це відкриває шлях до
/// SQL-ін'єкцій. Замість цього використовують іменовані параметри (@param):
/// драйвер сам підставляє й екранує значення.
/// </summary>
internal sealed class ParameterizedQueryExample : IExample
{
    public string Title => "Параметри запиту (SqlParameter, OUTPUT-параметр)";

    public async Task RunAsync()
    {
        await Db.SeedAsync();

        using SqlConnection connection = new(Db.DatabaseConnectionString);
        await connection.OpenAsync();

        // @title, @author, @pageCount — вхідні параметри (заповнювачі в тексті SQL).
        // @newId — вихідний (OUTPUT) параметр: сервер поверне в нього згенерований Id.
        const string sql = @"
            INSERT INTO Books (Title, Author, PageCount)
            VALUES (@title, @author, @pageCount);
            SET @newId = SCOPE_IDENTITY();";

        using SqlCommand insert = new(sql, connection);
        insert.Parameters.AddWithValue("@title", "Jane Eyre");
        insert.Parameters.AddWithValue("@author", "Charlotte Brontë");
        insert.Parameters.AddWithValue("@pageCount", 500);

        SqlParameter newIdParameter = new("@newId", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };
        insert.Parameters.Add(newIdParameter);

        int rowsAdded = await insert.ExecuteNonQueryAsync();
        Console.WriteLine($"Додано рядків: {rowsAdded}");
        Console.WriteLine($"Id нової книги (з OUTPUT-параметра): {newIdParameter.Value}");

        Console.WriteLine();

        // Той самий підхід — для безпечного пошуку. Навіть якщо у значенні будуть
        // лапки чи "; DROP TABLE ..." — це залишиться просто текстом для пошуку.
        using SqlCommand search = new(
            "SELECT COUNT(*) FROM Books WHERE Author = @author;", connection);
        search.Parameters.AddWithValue("@author", "Charlotte Brontë");

        Console.WriteLine($"Книг цього автора в базі: {await search.ExecuteScalarAsync()}");
    }
}
