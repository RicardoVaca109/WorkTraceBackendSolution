using AutoMapper;
using MongoDB.Bson;
using WorkTrace.Application.DTOs.ServiceMgmtDTO.Management;
using WorkTrace.Application.Repositories;
using WorkTrace.Application.Services;
using WorkTrace.Data.Models;

namespace WorkTrace.Logic.Services;

public class ServiceandInstallationService(IInstallationStepRepository _installationStepRepository, IServiceRepository _serviceRepository, IMapper _mapper) : IServiceandInstallationService
{
    public async Task<ServiceInformationResponse?> GetByIdAsync(string id)
    {
        var serviceById = await _serviceRepository.GetAsync(id);
        if (serviceById == null) throw new Exception("Service not found");
        return _mapper.Map<ServiceInformationResponse>(serviceById);
    }

    public async Task<ServiceInformationResponse> CreateServiceWithStepAsync(CreateServiceRequest request)
    {
        var installationSteps = new List<InstallationStep>();

        foreach (var stepRequest in request.InstallationSteps)
        {
            var step = _mapper.Map<InstallationStep>(stepRequest);
            await _installationStepRepository.CreateAsync(step);
            installationSteps.Add(step);
        }
        var stepsIds = installationSteps.Select(s => new ObjectId(s.Id)).ToList();

        var service = new Service
        {
            Name = request.Name,
            Description = request.Description,
            InstallationSteps = stepsIds
        };
        await _serviceRepository.CreateAsync(service);

        var response = _mapper.Map<ServiceInformationResponse>(service);
        return response;
    }
}