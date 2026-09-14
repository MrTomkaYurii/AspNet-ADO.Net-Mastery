namespace AdoNet.ThreeLayer.Dapper.DAL.Students;

/// <summary>
/// Сутність рівня даних — один рядок таблиці dbo.Students.
/// "Чистий" клас без логіки: Dapper заповнює його властивості сам,
/// зіставляючи назви стовпців результату запиту з назвами властивостей (за назвою, без урахування регістру).
/// Саме тому в StudentRepository немає жодного ручного Map()/GetOrdinal(), як у "чистому" ADO.NET.
/// </summary>
public sealed class Student
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
