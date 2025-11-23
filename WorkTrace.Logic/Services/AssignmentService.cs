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

    public async Task<List<ClientHistoryResponse>> GetClientHistoryAsync(string clientId)
    {
        var rawData = await _assignmentRepository.GetClientAssignmentRawAsync(clientId);

        var mapResult = rawData.Select(doc =>
        {
            DateTime assigned = doc.Contains("AssignedDate") && !doc["AssignedDate"].IsBsonNull
                ? doc["AssignedDate"].ToUniversalTime()
                : DateTime.MinValue;

            DateTime? co = null;
            if (doc.Contains("CheckOut") && !doc["CheckOut"].IsBsonNull)
                co = doc["CheckOut"].ToUniversalTime();

            return new ClientHistoryResponse
        {
            Service = doc["Service"].AsString,
                Date = assigned == DateTime.MinValue ? "" : assigned.ToString("dd-MM-yyyy"),
                Time = assigned == DateTime.MinValue ? "" : assigned.ToString("HH:mm"),
                CheckOutDate = co == null ? "" : co.Value.ToString("dd-MM-yyyy"),
                CheckOutTime = co == null ? "" : co.Value.ToString("HH:mm"),
            Status = doc["Status"].AsString,
            Address = doc["Address"].AsString,
                Users = doc["Users"]
                    .AsBsonArray
                    .Select(u => u.AsString)
                    .ToList()
            };
        })
        .ToList();
        return mapResult;
    }

    public async Task<AssignmentResponse> UpdateAssignmentAsync(string id, UpdateAssignmentWebRequest request)
    {
        var assignment = await _assignmentRepository.GetAsync(id) ?? throw new Exception("Assignment not found");

        if (!string.IsNullOrEmpty(request.Address))
        {
            assignment.DestinationLocation = await _geocodingService.GetGeoPointAsync(request.Address);
        }

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