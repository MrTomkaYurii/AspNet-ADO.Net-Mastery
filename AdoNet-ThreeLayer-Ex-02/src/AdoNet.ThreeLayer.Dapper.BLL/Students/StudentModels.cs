namespace AdoNet.ThreeLayer.Dapper.BLL.Students;

/// <summary>
/// DTO (Data Transfer Object) — те, що BLL віддає назовні, у шар API.
/// Тримається окремо від сутності DAL: клієнт не залежить від структури таблиці.
/// </summary>
public sealed record StudentDto(int Id, string Name, string Email);

/// <summary>Дані для створення студента, які приходять із шару API.</summary>
public sealed record CreateStudentDto(string Name, string Email);
