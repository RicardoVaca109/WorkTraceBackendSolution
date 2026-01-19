using WorkTrace.Application.DTOs.TakenRequirementDTO;

namespace WorkTrace.Application.Services;

public interface ITakenRequirementService
{
    Task<List<TakenRequirementInformationResponse>> GetAllAsync();
    Task<TakenRequirementInformationResponse> GetByIdAsync(string id);
    Task<TakenRequirementInformationResponse> CreateAsync(CreateTakenRequirementRequest request);
    Task<TakenRequirementInformationResponse> UpdateAsync(string id, UpdateTakenRequirementRequest request);
    Task<List<TakenRequirementWithClientResponse>> GetByUserAndDateRangeAsync(string userId, DateTime start, DateTime end);
    Task<List<TakenRequirementWithClientResponse>> GetByDate(DateTime start, DateTime end);
}