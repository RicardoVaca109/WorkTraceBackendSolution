using WorkTrace.Application.DTOs.TakenRequirementDTO;

namespace WorkTrace.Application.Services
{
    public interface ITakenRequirementService
    {
        Task<List<TakenRequirementInformationResponse>> GetAllAsync();
        Task<TakenRequirementInformationResponse> GetByIdAsync(string id);
        Task<TakenRequirementInformationResponse> CreateAsync(CreateTakenRequirementRequest request);
        Task<TakenRequirementInformationResponse> UpdateAsync(UpdateTakenRequirementRequest request);
        Task<List<TakenRequirementInformationResponse>> GetByUserAndDateRangeAsync(string userId, DateTime start, DateTime end);
    }
}