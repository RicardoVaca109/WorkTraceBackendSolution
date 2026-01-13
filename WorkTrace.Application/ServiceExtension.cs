using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using WorkTrace.Application.DTOs.AssignmentDTO.Management;
using WorkTrace.Application.DTOs.AssignmentDTO.Mobile;
using WorkTrace.Application.DTOs.ClientDTO.Information;
using WorkTrace.Application.DTOs.FormTemplateDTO.Information;
using WorkTrace.Application.DTOs.ServiceMgmtDTO.Management;
using WorkTrace.Application.DTOs.StatusDTO.Information;
using WorkTrace.Application.DTOs.TakenRequirementDTO;
using WorkTrace.Application.DTOs.UserDTO.Information;
using WorkTrace.Application.DTOs.UserDTO.Login;
using WorkTrace.Application.Profiles;

namespace WorkTrace.Application;

public static class ServiceExtension
{
    public static void AddApplicationServices(this IServiceCollection services)
    {

        services.AddFluentValidationAutoValidation()
        //Assignment Validations
                .AddValidatorsFromAssemblyContaining<CreateAssignmentValidator>()
                .AddValidatorsFromAssemblyContaining<UpdateAssignmentWebValidator>()
                .AddValidatorsFromAssemblyContaining<FinishAssignmentValidator>()
                .AddValidatorsFromAssemblyContaining<StartAssignmentValidator>()
                .AddValidatorsFromAssemblyContaining<UpdateAssignmentMobileValidator>()
                .AddValidatorsFromAssemblyContaining<UpdateLocationValidator>()
                .AddValidatorsFromAssemblyContaining<UpdateProgressValidator>()
        //Client Validations
                .AddValidatorsFromAssemblyContaining<CreateClientValidator>()
                .AddValidatorsFromAssemblyContaining<UpdateClientValidator>()
        //FormTemplate and Question Validators
                .AddValidatorsFromAssemblyContaining<CreateFormQuestionRequestValidator>()
                .AddValidatorsFromAssemblyContaining<UpdateFormQuestionsRequestValidator>()
                .AddValidatorsFromAssemblyContaining<CreateFormTemplateRequestValidator>()
                .AddValidatorsFromAssemblyContaining<UpdateFormTemplateRequestValidator>()
        //User Validations
                .AddValidatorsFromAssemblyContaining<CreateUserValidator>()
                .AddValidatorsFromAssemblyContaining<LoginValidator>()
                .AddValidatorsFromAssemblyContaining<UpdateUserValidator>()
        //Service and installationstep Validations
                .AddValidatorsFromAssemblyContaining<InstallationStepValidator>()
                .AddValidatorsFromAssemblyContaining<ServiceValidator>()
                .AddValidatorsFromAssemblyContaining<UpdateInstallationStepValidator>()
                .AddValidatorsFromAssemblyContaining<UpdateServiceValidator>()
        //Status Validations
                .AddValidatorsFromAssemblyContaining<CreateStatusValidator>()
                .AddValidatorsFromAssemblyContaining<UpdateClientValidator>()
        //Taken Requirements Validators
                .AddValidatorsFromAssemblyContaining<CreateTakenRequirementRequestValidator>()
                .AddValidatorsFromAssemblyContaining<UpdateTakenRequirementRequestValidator>();

        //Automapper
        services.AddAutoMapper(cfg => { }, typeof(UserProfile).Assembly);
        services.AddAutoMapper(cfg => { }, typeof(ServiceProfile).Assembly);
        services.AddAutoMapper(cfg => { }, typeof(ClientProfile).Assembly);
        services.AddAutoMapper(cfg => { }, typeof(StatusProfile).Assembly);
        services.AddAutoMapper(cfg => { }, typeof(AssignmentProfile).Assembly);
        //services.AddAutoMapper(cfg => { }, typeof(AssignmentEvaluationProfile).Assembly);
        services.AddAutoMapper(cfg => { }, typeof(FormTemplateProfile).Assembly);
        services.AddAutoMapper(cfg => { }, typeof(TakenRequirementProfile).Assembly);
    }
}