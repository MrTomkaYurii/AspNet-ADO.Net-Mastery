using AdoNet.ThreeLayer.DAL.Infrastructure;
using AdoNet.ThreeLayer.DAL.Students;
using Microsoft.Extensions.DependencyInjection;

namespace AdoNet.ThreeLayer.DAL;

/// <summary>
/// Реєстрація сервісів шару даних. Викликається з Program.cs шару API.
/// Так шар DAL сам вирішує, як зареєструвати свої класи, а API лише передає рядок підключення.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddDataAccessLayer(this IServiceCollection services, string connectionString)
    {
        services.AddScoped<IStudentRepository>(_ => new StudentRepository(connectionString));
        services.AddSingleton(new DatabaseBootstrapper(connectionString));
        return services;
    }
}
