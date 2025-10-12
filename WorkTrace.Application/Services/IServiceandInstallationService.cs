using WorkTrace.Application.DTOs.ServiceMgmtDTO.Management;

namespace WorkTrace.Application.Services;

public interface IServiceandInstallationService
{
    Task<ServiceInformationResponse?> GetByIdAsync(string id);
    Task<ServiceInformationResponse> CreateServiceWithStepAsync(CreateServiceRequest request);
}