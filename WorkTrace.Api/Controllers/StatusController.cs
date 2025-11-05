using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkTrace.Application.DTOs.StatusDTO.Information;
using WorkTrace.Application.Services;

namespace WorkTrace.Api.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class StatusController(IStatusService statusService) : ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<List<StatusInformationResponse>> GetAll() =>
        await statusService.GetAllAsync();

    [Authorize]
    [HttpPost]
    public async Task<StatusInformationResponse> Create(CreateStatusRequest request) =>
        await statusService.CreateStatusAsync(request);

    [Authorize]
    [HttpPut]
    public async Task<StatusInformationResponse> Update(string id, UpdateStatusRequest request) =>
        await statusService.UpdateStatusAsync(id, request);

    [Authorize]
    [HttpPut]
    public async Task<IActionResult> DeactivateStatus (string id)
    {
        var succes = await statusService.SetStatusInactive(id);
        if(!succes) return NotFound("Status Not Found or Inactive");
        return Ok("Status set to inactive succesfully");
    }
}