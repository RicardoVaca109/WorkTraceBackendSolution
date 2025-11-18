using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkTrace.Application.DTOs.ClientDTO.Information;
using WorkTrace.Application.Services;

namespace WorkTrace.Api.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class ClientController(IClientService clientService) : ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<ClientInformationResponse>>> GetAll()
    {
        var clientsInSystem = await clientService.GetAllAsync();
        return Ok(clientsInSystem);
    }

    [Authorize]
    [HttpPost]
    public async Task<ClientInformationResponse> Create(CreateClientRequest request) =>
        await clientService.CreateClientAsync(request);

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<ClientInformationResponse>> Update(string id, [FromBody] UpdateClientRequest request)
    {
        var updateClient = await clientService.UpdateClientAsync(id, request);
        return Ok(updateClient);
    }
}