﻿using Microsoft.Extensions.DependencyInjection;
using WorkTrace.Application.Repositories;
using WorkTrace.Repositories.Repositories;

namespace WorkTrace.Repositories;

public static class ServiceExtension
{
    public static void AddRepositoriesServices(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IStatusRepository, StatusRepository>();
        services.AddScoped<IServiceRepository, ServiceRepository>();

    }
}
