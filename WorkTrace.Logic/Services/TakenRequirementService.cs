using AutoMapper;
using WorkTrace.Application.DTOs.TakenRequirementDTO;
using WorkTrace.Application.Repositories;
using WorkTrace.Application.Services;
using WorkTrace.Data.Models;

namespace WorkTrace.Logic.Services;

public class TakenRequirementService (
    ITakenRequirementRepository _takenRequirementRepository,
    IUserRepository _userRepository,
    IClientRepository _clientRepository,
    IMapper _mapper) : ITakenRequirementService
{
    public async Task<List<TakenRequirementInformationResponse>> GetAllAsync()
    {
        var requirements = await _takenRequirementRepository.GetAsync();
        return _mapper.Map<List<TakenRequirementInformationResponse>>(requirements);
    }

    public async Task<TakenRequirementInformationResponse> GetByIdAsync(string id)
    {
        var requirement = await _takenRequirementRepository.GetAsync(id)
            ?? throw new Exception("Registro de requerimiento no encontrado");

        return _mapper.Map<TakenRequirementInformationResponse>(requirement);
    }

    public async Task<TakenRequirementInformationResponse> CreateAsync(
        CreateTakenRequirementRequest request)
    {
        var user = await _userRepository.GetAsync(request.UserId);
        if (user is null)
            throw new Exception("Usuario no encontrado");

        if (!string.IsNullOrWhiteSpace(request.ClientId))
        {
            var client = await _clientRepository.GetAsync(request.ClientId);
            if (client is null)
                throw new Exception("Cliente no encontrado");
        }

        var entity = _mapper.Map<TakenRequirement>(request);

        entity.Date = DateTime.UtcNow;

        await _takenRequirementRepository.CreateAsync(entity);

        return _mapper.Map<TakenRequirementInformationResponse>(entity);
    }

    public async Task<TakenRequirementInformationResponse> UpdateAsync(
        UpdateTakenRequirementRequest request)
    {
        var requirement = await _takenRequirementRepository.GetAsync(request.Id);
        if (requirement is null)
            throw new Exception("Registro de requerimiento no encontrado");

        if (!string.IsNullOrWhiteSpace(request.ClientId))
        {
            var client = await _clientRepository.GetAsync(request.ClientId);
            if (client is null)
                throw new Exception("Cliente no encontrado");
        }

        _mapper.Map(request, requirement);

        await _takenRequirementRepository.UpdateAsync(requirement.Id, requirement);

        return _mapper.Map<TakenRequirementInformationResponse>(requirement);
    }
}