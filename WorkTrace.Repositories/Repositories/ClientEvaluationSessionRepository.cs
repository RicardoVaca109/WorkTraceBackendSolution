using MongoDB.Driver;
using WorkTrace.Application.Repositories;
using WorkTrace.Data;
using WorkTrace.Data.Models;

namespace WorkTrace.Repositories.Repositories;

public class ClientEvaluationSessionRepository : GenericRepository<ClientEvaluationSession>, IClientEvaluationSessionRepository
{
    public ClientEvaluationSessionRepository(WorkTraceContext context)
    {
        Collection = context
            .GetCollection<ClientEvaluationSession>("clientEvaluationSessions");
    }

    public async Task<ClientEvaluationSession?> GetByTokenAsync(string token)
    {
        return await Collection.Find(x =>
            x.Token == token &&
            !x.IsUsed &&
            x.ExpiresAt > DateTime.UtcNow
        ).FirstOrDefaultAsync();
    }

    public async Task MarkAsUsedAsync(string id)
    {
        var filter = Builders<ClientEvaluationSession>
            .Filter.Eq(x => x.Id, id);

        var update = Builders<ClientEvaluationSession>
            .Update.Set(x => x.IsUsed, true);

        await Collection.UpdateOneAsync(filter, update);
    }
}