using AdoNet_Explanation_Ex01.Infrastructure;

namespace AdoNet_Explanation_Ex01.Examples;

/// <summary>
/// Приклад 10. Прибирання: видалення бази даних (DROP DATABASE).
/// Корисно, щоб запустити демонстрацію "з чистого аркуша".
/// </summary>
internal sealed class DropDatabaseExample : IExample
{
    public string Title => "Видалення бази даних (DROP DATABASE)";

    public async Task RunAsync()
    {
        await Db.DropDatabaseAsync();

        Console.WriteLine($"Базу даних '{Db.DatabaseName}' видалено (якщо вона існувала).");
        Console.WriteLine("Будь-який наступний приклад, якому потрібні дані, створить її заново.");
    }
}
