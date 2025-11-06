using AutoMapper;
using MongoDB.Bson;
using WorkTrace.Application.DTOs.ServiceMgmtDTO.Management;
using WorkTrace.Application.Repositories;
using WorkTrace.Application.Services;
using WorkTrace.Data.Models;

namespace WorkTrace.Logic.Services;

public class ServiceandInstallationService(IInstallationStepRepository _installationStepRepository, IServiceRepository _serviceRepository, IMapper _mapper) : IServiceandInstallationService
{
    public async Task<List<ServiceInformationResponse>> GetAllAsync()
    {
        var services = await _serviceRepository.GetAsync();
        var response = new List<ServiceInformationResponse>();

        foreach (var service in services)
        {
            var serviceResponse = _mapper.Map<ServiceInformationResponse>(service);

            // Populate installation steps with full details
            var installationSteps = new List<InstallationStepInformationResponse>();
            foreach (var stepId in service.InstallationSteps)
            {
                var step = await _installationStepRepository.GetAsync(stepId.ToString());
                if (step != null)
                {
                    installationSteps.Add(_mapper.Map<InstallationStepInformationResponse>(step));
                }
            }

            serviceResponse.InstallationSteps = installationSteps.OrderBy(s => s.Steps).ToList();
            response.Add(serviceResponse);
        }

        return response;
    }

    public async Task<ServiceInformationResponse?> GetByIdAsync(string id)
    {
        var serviceById = await _serviceRepository.GetAsync(id);
        if (serviceById == null) throw new Exception("Service not found");

        var serviceResponse = _mapper.Map<ServiceInformationResponse>(serviceById);

        // Populate installation steps with full details
        var installationSteps = new List<InstallationStepInformationResponse>();
        foreach (var stepId in serviceById.InstallationSteps)
        {
            var step = await _installationStepRepository.GetAsync(stepId.ToString());
            if (step != null)
            {
                installationSteps.Add(_mapper.Map<InstallationStepInformationResponse>(step));
            }
        }

        serviceResponse.InstallationSteps = installationSteps.OrderBy(s => s.Steps).ToList();

        return serviceResponse;
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
        response.InstallationSteps = installationSteps
            .OrderBy(s => s.Steps)
            .Select(s => _mapper.Map<InstallationStepInformationResponse>(s))
            .ToList();

        return response;
    }

    public async Task<ServiceInformationResponse> UpdateServiceAsync(UpdateServiceRequest request)
    {
        var service = await _serviceRepository.GetAsync(request.Id);
        if (service == null) throw new Exception("Service not found");

        service.Name = request.Name;
        service.Description = request.Description;

        await _serviceRepository.UpdateAsync(request.Id, service);

        return await GetByIdAsync(request.Id);
    }

    public async Task<InstallationStepInformationResponse> UpdateInstallationStepAsync(UpdateInstallationStepRequest request)
    {
        var step = await _installationStepRepository.GetAsync(request.Id);
        if (step == null) throw new Exception("Installation step not found");

        step.Steps = request.Steps;
        step.Description = request.Description;

        await _installationStepRepository.UpdateAsync(request.Id, step);

        return _mapper.Map<InstallationStepInformationResponse>(step);
    }
}
