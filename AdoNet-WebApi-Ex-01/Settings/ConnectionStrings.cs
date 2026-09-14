namespace LearnAdoDotnet.Settings
{
    /// <summary>
    /// Клас-контейнер для рядків підключення до БД.
    /// Заповнюється з секції "ConnectionStrings" у appsettings.json
    /// через механізм прив'язки конфігурації (Configuration.Bind).
    /// </summary>
    public class ConnectionStrings
    {
        // Рядок підключення за замовчуванням (nullable — може бути відсутнім у конфігурації).
        public string? DefaultConnection { get; set; }
    }
}