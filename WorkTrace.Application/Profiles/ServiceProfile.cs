using AutoMapper;
using WorkTrace.Application.DTOs.ServiceMgmtDTO.Management;
using WorkTrace.Data.Models;

namespace WorkTrace.Application.Profiles;

public class ServiceProfile : Profile
{
    public ServiceProfile()
    {
        CreateMap<CreateInstallationStepRequest, InstallationStep>();
        CreateMap<CreateServiceRequest, Service>();
        CreateMap<Service, ServiceInformationResponse>()
            .ForMember(dest => dest.InstallationSteps, opt => opt.MapFrom(src => src.InstallationSteps.Select(x => x.ToString())));
    }
}