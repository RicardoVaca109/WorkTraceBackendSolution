using AutoMapper;
using MongoDB.Bson;
using WorkTrace.Application.DTOs.ClientDTO.Information;
using WorkTrace.Application.DTOs.TakenRequirementDTO;
using WorkTrace.Application.DTOs.UserDTO.Information;
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

    public async Task<List<TakenRequirementWithClientResponse>>GetByUserAndDateRangeAsync(string userId, DateTime start, DateTime end)
    {
        var requirements =
            await _takenRequirementRepository
                .GetByDateUserTakenRequirements(userId, start, end);

        var result = new List<TakenRequirementWithClientResponse>();

        foreach (var req in requirements)
        {
            var dto = _mapper.Map<TakenRequirementWithClientResponse>(req);

            if (req.Client.HasValue)
            {
                var client = await _clientRepository.GetAsync(req.Client.Value.ToString());
                if (client != null)
                {
                    dto.Client = new ClientInformationResponse
                    {
                        Id = client.Id.ToString(),
                        DocumentNumber = client.DocumentNumber,
                        FullName = client.FullName,
                        Email = client.Email,
                        PhoneNumber = client.PhoneNumber
                    };
                }
            }

            result.Add(dto);
        }

        return result;
    }

    public async Task<List<TakenRequirementUserAndClientResponse>> GetByDate(DateTime start, DateTime end)
    {
        var requirements = await _takenRequirementRepository.GetByDate(start, end);
        var result = new List<TakenRequirementUserAndClientResponse>();

        foreach (var req in requirements)
        {
            var dto = _mapper.Map<TakenRequirementUserAndClientResponse>(req);

            var user = await _userRepository.GetAsync(req.User.ToString());
            if (user != null)
            {
                dto.UserId = new UserInformationResponse
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    DocumentNumber = user.DocumentNumber,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Role = user.Role,
                    IsActive = user.IsActive
                };
            }
            if (req.Client.HasValue)
            {
                var client = await _clientRepository.GetAsync(req.Client.Value.ToString());
                if (client != null)
                {
                    dto.Client = new ClientInformationResponse
                    {
                        Id = client.Id,
                        FullName = client.FullName,
                        DocumentNumber = client.DocumentNumber,
                        Email = client.Email,
                        PhoneNumber = client.PhoneNumber
                    };
                }
            }
            result.Add(dto);
        }
        return result;
    }
}