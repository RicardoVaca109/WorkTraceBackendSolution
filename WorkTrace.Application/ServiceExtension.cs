using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using WorkTrace.Application.DTOs.ClientDTO.Information;
using WorkTrace.Application.DTOs.ServiceMgmtDTO.Management;
using WorkTrace.Application.DTOs.UserDTO.Information;
using WorkTrace.Application.DTOs.UserDTO.Login;
using WorkTrace.Application.Profiles;

namespace WorkTrace.Application;

public static class ServiceExtension
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        
        services.AddFluentValidationAutoValidation()
        //Client Validations
                .AddValidatorsFromAssemblyContaining<CreateClientValidator>()
                .AddValidatorsFromAssemblyContaining<UpdateClientValidator>()
        //User Validations
                .AddValidatorsFromAssemblyContaining<CreateUserValidator>()
                .AddValidatorsFromAssemblyContaining<LoginValidator>()
                .AddValidatorsFromAssemblyContaining<UpdateUserValidator>()
        //Service and installationstep Validations
                .AddValidatorsFromAssemblyContaining<InstallationStepValidator>()
                .AddValidatorsFromAssemblyContaining<ServiceValidator>();

        //Automapper
        services.AddAutoMapper(cfg => { }, typeof(UserProfile).Assembly);
        services.AddAutoMapper(cfg => { }, typeof(ServiceProfile).Assembly);
        services.AddAutoMapper(cfg => { }, typeof(ClientProfile).Assembly);
    }
}