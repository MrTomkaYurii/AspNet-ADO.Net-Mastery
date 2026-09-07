# AdoNet-Explanation-Ex01

Консольна програма-довідник з **ADO.NET** (`System.Data.SqlClient` + SQL Server).
Кожна можливість ADO.NET винесена в окремий приклад; приклади запускаються з меню.

## Як запустити

1. Вкажіть робочий рядок підключення до SQL Server у [`appsettings.json`](appsettings.json),
   ключ `ConnectionStrings:MsSqlConnectionString`.
2. Запустіть програму:

   ```bash
   dotnet run --project AdoNet-Explanation-Ex01.csproj
   ```

3. Оберіть номер прикладу в меню. Після кожного прикладу — `Enter`, щоб повернутись назад. `0` — вихід.

Демонстраційна база `AdoNet_Explanation_Ex01` і таблиця `Books` створюються автоматично.
Приклади самодостатні — їх можна запускати в будь-якому порядку.

## Приклади

| № | Приклад | Що показує |
|---|---------|------------|
| 1 | Підключення до SQL Server | `SqlConnection`, `OpenAsync`, властивості з'єднання, пул |
| 2 | Створення бази й таблиці | DDL (`CREATE DATABASE` / `CREATE TABLE`) через `ExecuteNonQueryAsync` |
| 3 | Додавання рядків | `INSERT`, `ExecuteNonQuery` повертає кількість рядків |
| 4 | Зміна та видалення | `UPDATE` / `DELETE` і кількість зачеплених рядків |
| 5 | Читання курсором | `SqlDataReader` — читання вперед, по одному рядку |
| 6 | Одне значення | `ExecuteScalar` для `COUNT` / `MIN` / `AVG` |
| 7 | Параметри запиту | `SqlParameter`, захист від SQL-ін'єкцій, `OUTPUT`-параметр |
| 8 | Транзакції | `SqlTransaction` — `Commit` та `Rollback` при помилці |
| 9 | Відключена модель | `SqlDataAdapter` + `DataTable` + `SqlCommandBuilder` |
| 10 | Видалення бази | `DROP DATABASE` — почати демонстрацію "з нуля" |

## Структура проєкту

```
Program.cs                 меню: список прикладів + цикл вибору
Infrastructure/
  AppConfig.cs             читання appsettings.json
  Db.cs                    рядки підключення, EnsureCreated / Seed / Drop / PrintBooks
Examples/
  IExample.cs              спільний інтерфейс (Title + RunAsync)
  *Example.cs              по одному прикладу на файл
```

Щоб додати новий приклад: створіть клас із інтерфейсом `IExample` у `Examples/`
і додайте його в список `examples` у [`Program.cs`](Program.cs) — меню оновиться саме.
