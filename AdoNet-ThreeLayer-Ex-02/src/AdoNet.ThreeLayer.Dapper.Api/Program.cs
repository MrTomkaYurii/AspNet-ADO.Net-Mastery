using AdoNet.ThreeLayer.Dapper.BLL;
using AdoNet.ThreeLayer.Dapper.DAL;
using AdoNet.ThreeLayer.Dapper.DAL.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// --- Точка збірки: з'єднуємо три шари через DI ---

// Рядок підключення живе в appsettings.json і нікуди більше не потрапляє —
// шар API просто передає його в DAL. DAL усередині використовує Dapper поверх ADO.NET.
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("У appsettings.json немає ConnectionStrings:DefaultConnection.");

builder.Services.AddDataAccessLayer(connectionString);  // шар 1 (DAL, на Dapper)
builder.Services.AddBusinessLogicLayer();               // шар 2 (BLL)

// Речі, специфічні для HTTP-шару.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Готуємо базу для прикладу (створити базу/таблицю, додати тестові дані, якщо порожньо).
using (var scope = app.Services.CreateScope())
{
    await scope.ServiceProvider.GetRequiredService<DatabaseBootstrapper>().EnsureCreatedAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
