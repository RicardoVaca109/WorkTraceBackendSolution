using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using WorkTrace.Application.DTOs.UserDTO.Information;
using WorkTrace.Application.DTOs.UserDTO.Login;

namespace WorkTrace.Application;

public static class ServiceExtension
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<LoginValidator>()
                .AddValidatorsFromAssemblyContaining<UpdateUserValidator>();

    }

}