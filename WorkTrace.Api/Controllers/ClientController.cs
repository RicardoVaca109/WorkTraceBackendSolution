using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorkTrace.Application.DTOs.ClientDTO.Information;
using WorkTrace.Application.Services;

namespace WorkTrace.Api.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ClientController(IClientService _clientService) : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public async Task<List<ClientInformationResponse>> GetAll() =>
            await _clientService.GetAllAsync();

        [Authorize]
        [HttpPost]
        public async Task<ClientInformationResponse> Create(CreateClientRequest request) =>
            await _clientService.CreateClientAsync(request);

        [Authorize]
        [HttpPut]
        public async Task<ClientInformationResponse> Update(string id, UpdateClientRequest request) =>
            await _clientService.UpdateClientAsync(id, request);
    }
}