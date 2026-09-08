# AdoNet-ThreeLayer-Ex-01

Той самий приклад зі студентами, що й `AdoNet-WebApi-Ex-01`, але розкладений на
**тришарову архітектуру**. SQL-запити взяті з базового прикладу без змін.

```
┌─────────────────────────────┐
│  API  (AdoNet.ThreeLayer.Api)│  HTTP: контролери, коди відповіді, Swagger, DI
└──────────────┬──────────────┘
               ▼
┌─────────────────────────────┐
│  BLL  (AdoNet.ThreeLayer.BLL)│  бізнес-логіка: перевірки, сторінки, Entity → DTO
└──────────────┬──────────────┘
               ▼
┌─────────────────────────────┐
│  DAL  (AdoNet.ThreeLayer.DAL)│  ADO.NET: SqlConnection/SqlCommand/SqlDataReader, SQL
└──────────────┬──────────────┘
               ▼
          SQL Server
```

Усі три проєкти входять до кореневого рішення `AspNet-ADO.Net-Examples.sln`
(у теці рішення «AdoNet-ThreeLayer-Ex-01»).

Кожен шар знає лише про сусіда знизу:

| Шар | Проєкт | Відповідальність | Залежності |
|-----|--------|------------------|------------|
| **DAL** | `AdoNet.ThreeLayer.DAL` | Підключення до БД, текст SQL, читання рядків у сутність `Student`. Тут єдине місце з `Microsoft.Data.SqlClient`. | — |
| **BLL** | `AdoNet.ThreeLayer.BLL` | Перевірка вхідних даних, розрахунок зсуву сторінок, `Student` → `StudentDto`. | DAL |
| **API** | `AdoNet.ThreeLayer.Api` | Маршрути, HTTP-коди, Swagger, збірка всіх шарів через DI. | BLL, DAL |

## Запити ADO.NET (шар DAL, `StudentRepository`)

| Метод | SQL | Виклик ADO.NET |
|-------|-----|----------------|
| `GetPageAsync` | `SELECT * FROM Students ORDER BY Id OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY` | `ExecuteReaderAsync` |
| `CountAsync` | `SELECT COUNT(*) FROM Students` | **`ExecuteScalarAsync`** |
| `GetByIdAsync` | `SELECT * FROM Students WHERE Id = @Id` | `ExecuteReaderAsync` |
| `InsertAsync` | `INSERT INTO Students (Name, Email) VALUES (@Name, @Email)` | `ExecuteNonQueryAsync` |
| `DeleteAsync` | `DELETE FROM Students WHERE Id = @Id` | `ExecuteNonQueryAsync` |

## Як запустити

1. Вкажіть свій рядок підключення в
   [`src/AdoNet.ThreeLayer.Api/appsettings.json`](src/AdoNet.ThreeLayer.Api/appsettings.json)
   (`ConnectionStrings:DefaultConnection`).
2. Запустіть API (з кореня репозиторію):

   ```bash
   dotnet run --project AdoNet-ThreeLayer-Ex-01/src/AdoNet.ThreeLayer.Api
   ```

   або у Visual Studio виберіть `AdoNet.ThreeLayer.Api` як стартовий проєкт.

3. Відкриється Swagger UI: `http://localhost:5280/swagger`.

База `AdoNet_ThreeLayer_Ex_01`, таблиця `Students` і кілька тестових рядків
створюються автоматично під час старту (`DatabaseBootstrapper`).
Той самий скрипт — у [`db/schema.sql`](db/schema.sql).

## Ендпойнти (ті самі, що в базовому прикладі)

| Метод | Маршрут | Опис |
|-------|---------|------|
| `GET`    | `/Students?page=1&count=10` | сторінка студентів |
| `GET`    | `/Students/Count` | кількість (ExecuteScalar) |
| `GET`    | `/Students/{id}` | студент за Id або 404 |
| `POST`   | `/Students/AddStudent` | додати; 400, якщо не пройшла перевірка |
| `DELETE` | `/Students/DeleteStudents/{id}` | видалити; 404, якщо не знайдено |

Готові запити — у [`src/AdoNet.ThreeLayer.Api/AdoNet.ThreeLayer.Api.http`](src/AdoNet.ThreeLayer.Api/AdoNet.ThreeLayer.Api.http).
