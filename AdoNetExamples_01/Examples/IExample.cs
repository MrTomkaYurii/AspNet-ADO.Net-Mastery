namespace AdoNet_Explanation_Ex01.Examples;

/// <summary>
/// Спільний контракт для всіх прикладів.
/// Меню у Program.cs будується автоматично зі списку об'єктів IExample,
/// тому щоб додати новий приклад, достатньо створити клас із цим інтерфейсом
/// і дописати його в список.
/// </summary>
internal interface IExample
{
    /// <summary>Назва пункту у меню.</summary>
    string Title { get; }

    /// <summary>Запускає демонстрацію.</summary>
    Task RunAsync();
}
