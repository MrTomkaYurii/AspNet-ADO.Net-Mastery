using System.Data.SqlClient;
using AdoNet_Explanation_Ex01.Infrastructure;

namespace AdoNet_Explanation_Ex01.Examples;

/// <summary>
/// Приклад 8. Транзакції (SqlTransaction).
/// Транзакція об'єднує кілька команд в одну неподільну операцію: або виконуються
/// всі, або жодна. Якщо десь сталася помилка — Rollback повертає базу у стан "до".
/// </summary>
internal sealed class TransactionExample : IExample
{
    public string Title => "Транзакції (SqlTransaction: Commit / Rollback)";

    public async Task RunAsync()
    {
        await Db.SeedAsync();
        Console.WriteLine($"Книг на початку: {await CountBooksAsync()}");

        // --- Транзакція 1: обидві команди успішні -> Commit ---
        using (SqlConnection connection = new(Db.DatabaseConnectionString))
        {
            await connection.OpenAsync();
            using SqlTransaction transaction = connection.BeginTransaction();

            try
            {
                using SqlCommand command = connection.CreateCommand();
                command.Transaction = transaction; // команду обов'язково прив'язуємо до транзакції

                command.CommandText = "INSERT INTO Books (Title, Author, PageCount) VALUES (N'Book A', N'Author A', 100);";
                await command.ExecuteNonQueryAsync();

                command.CommandText = "INSERT INTO Books (Title, Author, PageCount) VALUES (N'Book B', N'Author B', 200);";
                await command.ExecuteNonQueryAsync();

                transaction.Commit();
                Console.WriteLine("Транзакція 1: Commit — обидві книги додано.");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"Транзакція 1: Rollback — {ex.Message}");
            }
        }
        Console.WriteLine($"Книг після транзакції 1: {await CountBooksAsync()}");
        Console.WriteLine();

        // --- Транзакція 2: перша команда успішна, друга падає -> Rollback ---
        using (SqlConnection connection = new(Db.DatabaseConnectionString))
        {
            await connection.OpenAsync();
            using SqlTransaction transaction = connection.BeginTransaction();

            try
            {
                using SqlCommand command = connection.CreateCommand();
                command.Transaction = transaction;

                command.CommandText = "INSERT INTO Books (Title, Author, PageCount) VALUES (N'Book C', N'Author C', 300);";
                await command.ExecuteNonQueryAsync();

                // Помилка навмисне: стовпця Genre в таблиці Books немає.
                command.CommandText = "INSERT INTO Books (Title, Genre) VALUES (N'Book D', N'Novel');";
                await command.ExecuteNonQueryAsync();

                transaction.Commit();
            }
            catch (SqlException ex)
            {
                transaction.Rollback();
                Console.WriteLine($"Транзакція 2: Rollback — {ex.Message}");
            }
        }
        Console.WriteLine($"Книг після транзакції 2: {await CountBooksAsync()} — не змінилось, 'Book C' теж відкотили.");
    }

    // Маленький помічник саме для цього прикладу.
    private static async Task<int> CountBooksAsync()
    {
        using SqlConnection connection = new(Db.DatabaseConnectionString);
        await connection.OpenAsync();

        using SqlCommand command = new("SELECT COUNT(*) FROM Books;", connection);
        return Convert.ToInt32(await command.ExecuteScalarAsync());
    }
}
