using WorkTrace.Data.Models;

namespace WorkTrace.Application.Repositories;

public interface ITakenRequirementRepository : IGenericRepository<TakenRequirement>
{
    Task<List<TakenRequirement>> GetByDateUserTakenRequirements(string userId, DateTime startDate, DateTime endDate);
    Task<List<TakenRequirement>> GetByDate(DateTime startDate, DateTime endDate);
}