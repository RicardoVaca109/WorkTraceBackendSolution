using WorkTrace.Application.DTOs.ClientDTO.Information;
using WorkTrace.Application.DTOs.UserDTO.Information;

namespace WorkTrace.Application.Services;

public interface IClientService
{
    Task<List<ClientInformationResponse>> GetAllAsync();
    Task<ClientInformationResponse> CreateClientAsync(CreateClientRequest clientCreate);
    Task<ClientInformationResponse> UpdateClientAsync(string id, UpdateClientRequest client);
}