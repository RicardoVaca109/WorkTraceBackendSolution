using AutoMapper;
using WorkTrace.Application.DTOs.AssignmentDTO.Management;
using WorkTrace.Application.Repositories;
using WorkTrace.Application.Services;
using WorkTrace.Data.Models;

namespace WorkTrace.Logic.Services;

public class AssignmentService(IAssignmentRepository _assignmentRepository, IClientRepository _clientRepository, IGeocodingService _geocodingService, IServiceRepository _serviceRepository, IStatusRepository _statusRepository, IUserRepository _userRepository, IMapper _mapper) : IAssignmentService
{
    public async Task<AssignmentResponse> CreateAssigmentAdminAsync(CreateAssignmentRequest assignmentRequest)
    {
        await ValidateExistance(assignmentRequest);

        var assignment = _mapper.Map<Assignment>(assignmentRequest);

        assignment.DestinationLocation = await _geocodingService.GetGeoPointAsync(assignmentRequest.Address);

        await _assignmentRepository.CreateAsync(assignment);
        return _mapper.Map<AssignmentResponse>(assignment);
    }

    public async Task<List<AssignmentResponse>> GetAllAsync()
    {
        var assignmentsInSystem = await _assignmentRepository.GetAsync();
        return _mapper.Map<List<AssignmentResponse>>(assignmentsInSystem);
    }

    public async Task<AssignmentResponse> GetByIdAsync(string id)
    {
        var assignmentById = await _assignmentRepository.GetAsync(id) ?? throw new Exception("Assignment not found");
        var response = _mapper.Map<AssignmentResponse>(assignmentById);
        return response;
    }

    public async Task<List<ClientHistoryResponse>> GetClientHistoryAsync(string cliendId)
    {
        var rawData = await _assignmentRepository.GetClientAssignmentRawAsync(cliendId);

        var mapResult = rawData.Select(doc => new ClientHistoryResponse
        {
            Service = doc["Service"].AsString,
            Date = doc.Contains("AssignedDate") && !doc["AssignedDate"].IsBsonNull ? doc["AssignedDate"].ToUniversalTime() : DateTime.MinValue,
            CheckOut = doc.Contains("CheckOut") && !doc["CheckOut"].IsBsonNull ? doc["CheckOut"].ToUniversalTime() : null,
            Status = doc["Status"].AsString,
            Address = doc["Address"].AsString,
            Users = [.. doc["Users"].AsBsonArray.Select(u => u.AsString)],
        }).ToList();
        return mapResult;
    }

    public async Task<AssignmentResponse> UpdateAssignmentAsync(string id, UpdateAssignmentWebRequest request)
    {
        var assignment = await _assignmentRepository.GetAsync(id) ?? throw new Exception("Assignment not found");

        if (!string.IsNullOrEmpty(request.Address))
        {
            assignment.DestinationLocation = await _geocodingService.GetGeoPointAsync(request.Address);
        }

        // Aplicar cambios usando AutoMapper (actualización parcial)
        _mapper.Map(request, assignment);

        await _assignmentRepository.UpdateAsync(id, assignment);

        return _mapper.Map<AssignmentResponse>(assignment);
    }

    public async Task ValidateExistance(CreateAssignmentRequest assignmentRequest)
    {
        var client = await _clientRepository.GetAsync(assignmentRequest.Client);
        if (client == null) throw new Exception("Client does not exist");

        var service = await _serviceRepository.GetAsync(assignmentRequest.Service);
        if (service == null) throw new Exception("Service does not exist");

        var status = await _statusRepository.GetAsync(assignmentRequest.Status);
        if (status == null) throw new Exception("Status does not exist");

        foreach (var userId in assignmentRequest.Users)
        {
            var user = await _userRepository.GetAsync(userId);
            if (user == null) throw new Exception($"User with ID {userId} does not exist");
        }
    }
}