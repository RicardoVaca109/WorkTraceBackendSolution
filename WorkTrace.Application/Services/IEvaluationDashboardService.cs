using WorkTrace.Application.DTOs.KPIsDTO;

namespace WorkTrace.Application.Services;

public interface IEvaluationDashboardService
{
    Task<EvaluationDashboardResponse> GetDashboardAsync(string userId, DateTime start, DateTime end);
}