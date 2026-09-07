using LearnAdoDotnet.Settings;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "API",
        Description = "Web API",
    });
});

var ConnectionStrings = new ConnectionStrings();

builder.Configuration.Bind(nameof(ConnectionStrings), ConnectionStrings);
builder.Services.AddSingleton(ConnectionStrings);
builder.Services.AddTransient<DataAccessRepository>();
builder.Services.AddTransient<StudentsService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
    app.UseAuthorization();
}

app.MapControllers();

app.Run();
