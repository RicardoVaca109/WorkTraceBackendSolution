using AutoMapper;
using WorkTrace.Application.DTOs.ClientDTO.Information;
using WorkTrace.Application.DTOs.UserDTO.Information;
using WorkTrace.Application.Repositories;
using WorkTrace.Application.Services;
using WorkTrace.Data.Models;

namespace WorkTrace.Logic.Services;

public class ClientService(IClientRepository _clientRepository, IMapper _mapper) : IClientService
{
    public async Task<List<ClientInformationResponse>> GetAllAsync()
    {
        var systemClients = await _clientRepository.GetAsync();
        return _mapper.Map<List<ClientInformationResponse>>(systemClients);
    }

    public async Task<ClientInformationResponse> CreateClientAsync(CreateClientRequest clientCreate)
    {
        var existingClient = await _clientRepository.GetByDocumentNumberAsync(clientCreate.DocumentNumber);
        if (existingClient != null) throw new Exception("There is already a client with this document number");

        
        var clientToDatabase = _mapper.Map<Client>(clientCreate);

        await _clientRepository.CreateAsync(clientToDatabase);
        return _mapper.Map<ClientInformationResponse>(clientToDatabase);
    }

    public async Task<ClientInformationResponse> UpdateClientAsync(string id, UpdateClientRequest client)
    {
        var clientToUpdate = await _clientRepository.GetAsync(id);
        if (clientToUpdate == null) throw new Exception("Client not found");

        clientToUpdate.FullName = string.IsNullOrWhiteSpace(client.FullName) ? clientToUpdate.FullName : client.FullName;
        clientToUpdate.DocumentNumber = string.IsNullOrWhiteSpace(client.DocumentNumber) ? clientToUpdate.DocumentNumber : client.DocumentNumber;
        clientToUpdate.PhoneNumber = string.IsNullOrWhiteSpace(client.PhoneNumber) ? clientToUpdate.PhoneNumber : client.PhoneNumber;
        clientToUpdate.Email = string.IsNullOrWhiteSpace(client.Email) ? clientToUpdate.Email : client.Email;

        await _clientRepository.UpdateAsync(id, clientToUpdate);
        return _mapper.Map<ClientInformationResponse>(clientToUpdate);
    }
}
