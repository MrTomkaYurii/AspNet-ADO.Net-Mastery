# AdoNet-ThreeLayer-Ex-02

Той самий приклад зі студентами й та сама **тришарова архітектура**, що й
у `AdoNet-ThreeLayer-Ex-01`, але шар DAL написаний не на "чистому" ADO.NET,
а на **Dapper поверх ADO.NET**.

Dapper — це мікро-ORM: набір extension-методів для `IDbConnection`
(`Query`, `QueryFirstOrDefault`, `Execute`, `ExecuteScalar`), які самі
створюють `SqlCommand`, підставляють параметри та мапують результат у
об'єкти. Він **не замінює** ADO.NET — з'єднання всередині все одно
`Microsoft.Data.SqlClient.SqlConnection`, Dapper лише прибирає рутину
навколо нього (ручний `SqlCommand`, цикл `while (reader.Read())`,
`GetOrdinal`/`GetString`/`GetInt32`).

```
┌──────────────────────────────────────┐
│  API  (AdoNet.ThreeLayer.Dapper.Api)  │  HTTP: контролери, коди відповіді, Swagger, DI
└──────────────────┬───────────────────┘
                    ▼
┌──────────────────────────────────────┐
│  BLL  (AdoNet.ThreeLayer.Dapper.BLL)  │  бізнес-логіка: перевірки, сторінки, Entity → DTO
└──────────────────┬───────────────────┘
                    ▼
┌──────────────────────────────────────┐
│  DAL  (AdoNet.ThreeLayer.Dapper.DAL)  │  ADO.NET + Dapper: SqlConnection + Query/Execute, SQL
└──────────────────┬───────────────────┘
                    ▼
               SQL Server
```

Усі три проєкти входять до кореневого рішення `AspNet-ADO.Net-Examples.sln`
(у теці рішення «AdoNet-ThreeLayer-Example04»).

Кожен шар знає лише про сусіда знизу:

| Шар | Проєкт | Відповідальність | Залежності |
|-----|--------|------------------|------------|
| **DAL** | `AdoNet.ThreeLayer.Dapper.DAL` | Підключення до БД, текст SQL, виклики Dapper, які самі мапують рядки в сутність `Student`. Тут єдине місце з `Microsoft.Data.SqlClient` і `Dapper`. | — |
| **BLL** | `AdoNet.ThreeLayer.Dapper.BLL` | Перевірка вхідних даних, розрахунок зсуву сторінок, `Student` → `StudentDto`. | DAL |
| **API** | `AdoNet.ThreeLayer.Dapper.Api` | Маршрути, HTTP-коди, Swagger, збірка всіх шарів через DI. | BLL, DAL |

## Чим DAL відрізняється від AdoNet-ThreeLayer-Ex-01

SQL-запити й контракт `IStudentRepository` — ідентичні. Різниця лише в
реалізації `StudentRepository`:

| Метод | SQL | "Чистий" ADO.NET (Ex-01) | ADO.NET + Dapper (тут) |
|-------|-----|---------------------------|--------------------------|
| `GetPageAsync` | `SELECT ... OFFSET/FETCH` | `SqlCommand` + `SqlDataReader` + цикл `while (reader.Read())` + `Map()` | `connection.QueryAsync<Student>(sql, parameters)` |
| `CountAsync` | `SELECT COUNT(*) FROM Students` | `ExecuteScalarAsync` + `Convert.ToInt32(result)` | `connection.ExecuteScalarAsync<int>(sql)` |
| `GetByIdAsync` | `SELECT * FROM Students WHERE Id = @Id` | `ExecuteReaderAsync` + `reader.Read() ? Map(reader) : null` | `connection.QueryFirstOrDefaultAsync<Student>(sql, parameters)` |
| `InsertAsync` | `INSERT INTO Students (Name, Email) VALUES (@Name, @Email)` | `ExecuteNonQueryAsync` | `connection.ExecuteAsync(sql, parameters)` |
| `DeleteAsync` | `DELETE FROM Students WHERE Id = @Id` | `ExecuteNonQueryAsync` | `connection.ExecuteAsync(sql, parameters)` |

Параметри в Dapper передаються анонімним об'єктом (`new { Id = id }`),
а не колекцією `SqlParameter` — Dapper сам зіставляє властивості об'єкта
з `@Id`/`@Name`/`@Email` у тексті запиту. Це такий самий захист від
SQL-ін'єкцій, як і ручні `SqlParameter` в Ex-01: значення завжди йдуть
окремим параметром, а не конкатенацією в рядок SQL.

## Як запустити

1. Вкажіть свій рядок підключення в
   [`src/AdoNet.ThreeLayer.Dapper.Api/appsettings.json`](src/AdoNet.ThreeLayer.Dapper.Api/appsettings.json)
   (`ConnectionStrings:DefaultConnection`).
2. Запустіть API (з кореня репозиторію):

   ```bash
   dotnet run --project AdoNet-ThreeLayer-Ex-02/src/AdoNet.ThreeLayer.Dapper.Api
   ```

   або у Visual Studio виберіть `AdoNet.ThreeLayer.Dapper.Api` як стартовий проєкт.

3. Відкриється Swagger UI: `http://localhost:5281/swagger`
   (порт інший, ніж у Ex-01, — щоб обидва приклади могли працювати одночасно).

База `AdoNet_ThreeLayer_Ex_02`, таблиця `Students` і кілька тестових рядків
створюються автоматично під час старту (`DatabaseBootstrapper`, теж переписаний
на Dapper). Той самий скрипт — у [`db/schema.sql`](db/schema.sql).

## Ендпойнти (ті самі, що в AdoNet-ThreeLayer-Ex-01)

| Метод | Маршрут | Опис |
|-------|---------|------|
| `GET`    | `/Students?page=1&count=10` | сторінка студентів |
| `GET`    | `/Students/Count` | кількість (`ExecuteScalarAsync<int>`) |
| `GET`    | `/Students/{id}` | студент за Id або 404 |
| `POST`   | `/Students/AddStudent` | додати; 400, якщо не пройшла перевірка |
| `DELETE` | `/Students/DeleteStudents/{id}` | видалити; 404, якщо не знайдено |

Готові запити — у
[`src/AdoNet.ThreeLayer.Dapper.Api/AdoNet.ThreeLayer.Dapper.Api.http`](src/AdoNet.ThreeLayer.Dapper.Api/AdoNet.ThreeLayer.Dapper.Api.http).
