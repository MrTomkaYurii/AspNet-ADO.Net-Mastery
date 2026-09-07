using Microsoft.Extensions.Configuration;

namespace AdoNet_Explanation_Ex01.Infrastructure;

/// <summary>
/// Читає файл appsettings.json.
/// ADO.NET сам не працює з конфігурацією, тому рядок підключення дістаємо
/// через Microsoft.Extensions.Configuration.
/// </summary>
internal static class AppConfig
{
    // Конфігурація будується один раз і кешується для всієї програми.
    private static readonly IConfigurationRoot Configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false)
        .Build();

    /// <summary>
    /// Рядок підключення до СЕРВЕРА (без конкретної бази — фактично до системної бази master).
    /// Його вистачає, щоб створити або видалити нашу демонстраційну базу.
    /// </summary>
    public static string ServerConnectionString =>
        Configuration.GetConnectionString("MsSqlConnectionString")
        ?? throw new InvalidOperationException(
            "У appsettings.json немає рядка підключення 'MsSqlConnectionString'.");
}
