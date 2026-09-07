using System.Data.SqlClient;
using AdoNet_Explanation_Ex01.Infrastructure;

namespace AdoNet_Explanation_Ex01.Examples;

/// <summary>
/// Приклад 1. Підключення до SQL Server.
/// SqlConnection — вхідна точка будь-якої роботи з ADO.NET. Рядок підключення
/// каже, ДЕ сервер, ДО ЯКОЇ бази під'єднуватись і ЯК автентифікуватись.
/// </summary>
internal sealed class ConnectionInfoExample : IExample
{
    public string Title => "Підключення до SQL Server (SqlConnection)";

    public async Task RunAsync()
    {
        // Створення об'єкта SqlConnection ще НЕ відкриває з'єднання —
        // це лише опис того, куди й як під'єднуватись.
        using SqlConnection connection = new(Db.ServerConnectionString);
        Console.WriteLine($"Стан до відкриття:    {connection.State}");   // Closed

        // OpenAsync фактично встановлює з'єднання із сервером.
        await connection.OpenAsync();
        Console.WriteLine($"Стан після відкриття: {connection.State}");   // Open

        Console.WriteLine();
        Console.WriteLine($"Сервер:            {connection.DataSource}");
        Console.WriteLine($"Версія SQL Server: {connection.ServerVersion}");
        Console.WriteLine($"Поточна база:      {connection.Database}");
        Console.WriteLine($"Робоча станція:    {connection.WorkstationId}");

        // Вихід із блоку using автоматично викличе Dispose:
        // з'єднання закриється й повернеться у пул підключень.
    }
}
