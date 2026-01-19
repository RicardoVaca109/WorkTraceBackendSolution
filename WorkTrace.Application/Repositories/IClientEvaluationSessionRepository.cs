using WorkTrace.Data.Models;

namespace WorkTrace.Application.Repositories;

public interface IClientEvaluationSessionRepository : IGenericRepository<ClientEvaluationSession>
{
    Task<ClientEvaluationSession?> GetByTokenAsync(string token);
    Task CreateAsync(ClientEvaluationSession session);
    Task MarkAsUsedAsync(string id);
}