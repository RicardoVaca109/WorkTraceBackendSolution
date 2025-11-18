using WorkTrace.Application.DTOs.StatusDTO.Information;

namespace WorkTrace.Application.Services;

public interface IStatusService
{
    Task<List<StatusInformationResponse>> GetAllAsync();
    Task<StatusInformationResponse> CreateStatusAsync(CreateStatusRequest statusCreate);
    Task<StatusInformationResponse> UpdateStatusAsync(string id, UpdateStatusRequest status);
    Task<bool> SetStatusInactive(string statusId);
}