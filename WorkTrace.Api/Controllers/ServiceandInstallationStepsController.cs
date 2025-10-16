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
    public async Task<ServiceInformationResponse> GetById(string id) =>
        await _serviceandInstallationService.GetByIdAsync(id);

    [Authorize]
    [HttpPost]
    public async Task<ServiceInformationResponse> Create(CreateServiceRequest request) =>
        await _serviceandInstallationService.CreateServiceWithStepAsync(request);
}