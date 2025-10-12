using Microsoft.Extensions.DependencyInjection;
using WorkTrace.Application.Services;
using WorkTrace.Logic.Services;

namespace WorkTrace.Logic;

public static class ServiceExtension
{
    public static void AddLogicServices(this IServiceCollection services)
    {
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IServiceandInstallationService, ServiceandInstallationService>();
    }
}