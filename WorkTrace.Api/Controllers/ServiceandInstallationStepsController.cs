using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkTrace.Application.DTOs.ServiceMgmtDTO.Management;
using WorkTrace.Application.Services;

namespace WorkTrace.Api.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class ServiceandInstallationStepsController(IServiceandInstallationService  _serviceandInstallationService) : ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<List<ServiceInformationResponse>> GetAll() =>
        await _serviceandInstallationService.GetAllAsync();
    [Authorize]
    [HttpGet]
    public async Task<ServiceInformationResponse> GetById(string id) =>
        await _serviceandInstallationService.GetByIdAsync(id);

    [Authorize]
    [HttpPost]
    public async Task<ServiceInformationResponse> Create(CreateServiceRequest request) =>
        await _serviceandInstallationService.CreateServiceWithStepAsync(request);

    [Authorize]
    [HttpPut]
    public async Task<ServiceInformationResponse> UpdateService(UpdateServiceRequest request) =>
        await _serviceandInstallationService.UpdateServiceAsync(request);
    [Authorize]
    [HttpPut]
    public async Task<InstallationStepInformationResponse> UpdateInstallationStep(UpdateInstallationStepRequest request) =>
        await _serviceandInstallationService.UpdateInstallationStepAsync(request);
}