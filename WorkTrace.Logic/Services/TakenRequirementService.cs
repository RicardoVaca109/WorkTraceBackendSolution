using AutoMapper;
using MongoDB.Bson;
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
    string id,
    UpdateTakenRequirementRequest request)
    {
        var requirement = await _takenRequirementRepository.GetAsync(id);
        if (requirement == null)
            throw new Exception("TakenRequirement no encontrado");

        requirement.Title = string.IsNullOrWhiteSpace(request.Title)
            ? requirement.Title
            : request.Title;

        requirement.Description = string.IsNullOrWhiteSpace(request.Description)
            ? requirement.Description
            : request.Description;

        if (request.ClientId == null)
        {
            requirement.Client = null;
        }
        else if (!string.IsNullOrWhiteSpace(request.ClientId))
        {
            requirement.Client = ObjectId.Parse(request.ClientId);
        }

        await _takenRequirementRepository.UpdateAsync(id, requirement);

        return _mapper.Map<TakenRequirementInformationResponse>(requirement);
    }

    public async Task<List<TakenRequirementInformationResponse>> GetByUserAndDateRangeAsync(string userId, DateTime start, DateTime end)
    {
        var user = await _userRepository.GetAsync(userId);
        if (user is null)
            throw new Exception("Usuario no encontrado");

        var data = await _takenRequirementRepository
            .GetByDateUserTakenRequirements(userId, start, end);

        var result = new List<TakenRequirementInformationResponse>();

        foreach (var item in data)
        {
            string? clientName = null;

            if (item.Client.HasValue)
            {
                var client = await _clientRepository.GetAsync(item.Client.Value.ToString());
                clientName = client?.FullName;
            }

            var dto = new TakenRequirementInformationResponse
            {
                Id = item.Id,
                UserId = item.User.ToString(),
                ClientId = item.Client?.ToString(),
                Date = item.Date.ToLocalTime(),
                Title = item.Title,
                Description = item.Description
            };

            result.Add(dto);
        }

        return result;
    }
}