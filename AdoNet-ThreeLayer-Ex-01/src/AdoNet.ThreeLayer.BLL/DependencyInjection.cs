using AdoNet.ThreeLayer.BLL.Students;
using Microsoft.Extensions.DependencyInjection;

namespace AdoNet.ThreeLayer.BLL;

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
