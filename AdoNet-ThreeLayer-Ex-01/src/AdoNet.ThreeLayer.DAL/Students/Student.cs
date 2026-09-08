namespace AdoNet.ThreeLayer.DAL.Students;

/// <summary>
/// Сутність рівня даних — один рядок таблиці dbo.Students.
/// Це "чистий" клас без логіки: DAL заповнює його з SqlDataReader.
/// </summary>
public sealed class Student
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
