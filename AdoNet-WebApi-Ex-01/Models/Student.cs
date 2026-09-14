namespace LearnAdoDotnet.Models
{
    /// <summary>
    /// Модель студента — відповідає рядку таблиці Students у базі даних.
    /// Використовується сервісом та репозиторієм для читання/запису даних.
    /// </summary>
    public class Student
    {
        // Первинний ключ (identity-стовпець у таблиці, генерується БД).
        public int Id { get; set; }

        // Ім'я студента (nullable через увімкнений Nullable-контекст).
        public string? Name { get; set; }

        // Електронна пошта студента.
        public string? Email { get; set; }
    }
}