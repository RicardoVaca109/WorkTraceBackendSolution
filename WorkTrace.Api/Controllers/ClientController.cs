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
    public async Task<List<ClientInformationResponse>> GetAll() =>
        await clientService.GetAllAsync();

    [Authorize]
    [HttpPost]
    public async Task<ClientInformationResponse> Create(CreateClientRequest request) =>
        await clientService.CreateClientAsync(request);

    [Authorize]
    [HttpPut]
    public async Task<ClientInformationResponse> Update(string id, UpdateClientRequest request) =>
        await clientService.UpdateClientAsync(id, request);
}