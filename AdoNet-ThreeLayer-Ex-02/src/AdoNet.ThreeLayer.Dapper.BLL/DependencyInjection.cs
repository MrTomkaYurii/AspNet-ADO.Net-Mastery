using AdoNet.ThreeLayer.Dapper.BLL.Students;
using Microsoft.Extensions.DependencyInjection;

namespace AdoNet.ThreeLayer.Dapper.BLL;

/// <summary>
/// Реєстрація сервісів шару бізнес-логіки. Викликається з Program.cs шару API.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services)
    {
        services.AddScoped<IStudentService, StudentService>();
        return services;
    }
}
