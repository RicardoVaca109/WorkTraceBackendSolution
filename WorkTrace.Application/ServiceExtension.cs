using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using WorkTrace.Application.DTOs.UserDTO.Information;
using WorkTrace.Application.DTOs.UserDTO.Login;
using WorkTrace.Application.Profiles;

namespace WorkTrace.Application;

public static class ServiceExtension
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation()
                .AddValidatorsFromAssemblyContaining<CreateUserValidator>()
                .AddValidatorsFromAssemblyContaining<LoginValidator>()
                .AddValidatorsFromAssemblyContaining<UpdateUserValidator>();

        //Automapper
        services.AddAutoMapper(cfg => { }, typeof(UserProfile).Assembly);
    }
}