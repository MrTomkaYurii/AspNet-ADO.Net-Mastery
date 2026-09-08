namespace AdoNet.ThreeLayer.BLL.Students;

/// <summary>
/// Кидається шаром BLL, коли вхідні дані не проходять перевірку бізнес-правил.
/// Шар API перехоплює цей виняток і повертає HTTP 400 (Bad Request).
/// </summary>
public sealed class ValidationException : Exception
{
    public ValidationException(string message) : base(message)
    {
    }
}
