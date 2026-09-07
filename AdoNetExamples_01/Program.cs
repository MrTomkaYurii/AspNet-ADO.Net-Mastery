using System.Text;
using AdoNet_Explanation_Ex01.Examples;

// ────────────────────────────────────────────────────────────────────────────────
//  ADO.NET — демонстраційна консольна програма.
//  Кожен приклад показує одну можливість ADO.NET і є самодостатнім:
//  його можна запускати в будь-якому порядку — потрібну базу/дані він створює сам.
//  Рядок підключення береться з appsettings.json (ключ "MsSqlConnectionString").
// ────────────────────────────────────────────────────────────────────────────────

// Щоб кирилиця й рамки коректно виводились у консолі.
try { Console.OutputEncoding = Encoding.UTF8; } catch { /* деякі термінали цього не дозволяють */ }

// Усі приклади в порядку зростання складності.
// Меню будується автоматично з цього списку — щоб додати приклад, допишіть рядок.
IReadOnlyList<IExample> examples = new IExample[]
{
    new ConnectionInfoExample(),      // 1
    new CreateSchemaExample(),        // 2
    new InsertDataExample(),          // 3
    new UpdateDeleteExample(),        // 4
    new DataReaderExample(),          // 5
    new ScalarQueryExample(),         // 6
    new ParameterizedQueryExample(),  // 7
    new TransactionExample(),         // 8
    new DataAdapterExample(),         // 9
    new DropDatabaseExample(),        // 10
};

while (true)
{
    ShowMenu(examples);

    Console.Write("Ваш вибір: ");
    string? choice = Console.ReadLine()?.Trim();
    Console.WriteLine();

    if (string.IsNullOrEmpty(choice))
    {
        continue;
    }

    if (choice is "0" or "q" or "Q" or "exit")
    {
        break;
    }

    if (!int.TryParse(choice, out int number) || number < 1 || number > examples.Count)
    {
        Print("Немає такого пункту меню. Спробуйте ще раз.", ConsoleColor.Yellow);
        continue;
    }

    IExample example = examples[number - 1];
    Print($"═════ {number}. {example.Title} ═════", ConsoleColor.Cyan);

    try
    {
        await example.RunAsync();
    }
    catch (Exception ex)
    {
        Print($"Помилка: {ex.Message}", ConsoleColor.Red);
        Print("Перевірте, що SQL Server запущено, а рядок підключення в appsettings.json правильний.",
            ConsoleColor.DarkGray);
    }

    Console.WriteLine();
    Console.Write("Натисніть Enter, щоб повернутися до меню...");
    Console.ReadLine();
}

Console.WriteLine("До зустрічі!");

// ───────────────────────────── допоміжні локальні методи ─────────────────────────

static void ShowMenu(IReadOnlyList<IExample> examples)
{
    Console.WriteLine();
    Console.WriteLine("==================================================");
    Console.WriteLine("   ADO.NET — демонстрація можливостей");
    Console.WriteLine("==================================================");
    for (int i = 0; i < examples.Count; i++)
    {
        Console.WriteLine($"  {i + 1,2}. {examples[i].Title}");
    }
    Console.WriteLine("   0. Вихід");
    Console.WriteLine();
}

static void Print(string text, ConsoleColor color)
{
    ConsoleColor previous = Console.ForegroundColor;
    Console.ForegroundColor = color;
    Console.WriteLine(text);
    Console.ForegroundColor = previous;
}
