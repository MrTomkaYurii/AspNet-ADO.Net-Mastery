using LearnAdoDotnet.Settings;
using Microsoft.OpenApi;

// Точка входу застосунку. WebApplicationBuilder збирає конфігурацію,
// DI-контейнер (реєстр сервісів) та налаштування хостингу.
var builder = WebApplication.CreateBuilder(args);

// --- Реєстрація сервісів у DI-контейнері ---

// Додаємо підтримку контролерів (атрибутна маршрутизація, модель-байндинг тощо).
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// Swagger/OpenAPI — автоматична документація та тестова UI-сторінка для API.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "API",
        Description = "Web API",
    });

    var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// Створюємо об'єкт налаштувань і заповнюємо його значеннями із секції
// "ConnectionStrings" файлу appsettings.json (nameof(ConnectionStrings) == "ConnectionStrings").
var ConnectionStrings = new ConnectionStrings();

builder.Configuration.Bind(nameof(ConnectionStrings), ConnectionStrings);

// Singleton — один екземпляр налаштувань на весь час роботи застосунку.
builder.Services.AddSingleton(ConnectionStrings);

// Transient — новий екземпляр створюється щоразу, коли його запитують.
// Репозиторій доступу до даних (ADO.NET) та сервіс бізнес-логіки для студентів.
builder.Services.AddTransient<DataAccessRepository>();
builder.Services.AddTransient<StudentsService>();

// Будуємо застосунок після того, як усі сервіси зареєстровані.
var app = builder.Build();

// --- Налаштування конвеєра обробки HTTP-запитів (middleware) ---
// Порядок виклику middleware важливий: запит проходить їх зверху вниз.
if (app.Environment.IsDevelopment())
{
    // У режимі розробки вмикаємо Swagger, щоб зручно тестувати ендпойнти.
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // У продакшені перенаправляємо HTTP -> HTTPS і вмикаємо авторизацію.
    app.UseHttpsRedirection();
    app.UseAuthorization();
}

// Редирект з кореневого маршруту на Swagger UI
app.MapGet("/", () => Results.Redirect("/swagger"));

// Зіставляємо маршрути з методами контролерів.
app.MapControllers();

// Запускаємо веб-сервер і блокуємо потік до зупинки застосунку.
app.Run();
