using MongoDB.Bson;
using MongoDB.Driver;
using System.Runtime.CompilerServices;
using WorkTrace.Application.Repositories;
using WorkTrace.Data;
using WorkTrace.Data.Models;

namespace WorkTrace.Repositories.Repositories;

public class TakenRequirementRepository : GenericRepository<TakenRequirement>, ITakenRequirementRepository
{
    public TakenRequirementRepository(WorkTraceContext context)
    {
        Collection = context.GetCollection<TakenRequirement>("takenRequirements");
    }

    public async Task<List<TakenRequirement>> GetByDateUserTakenRequirements(string userId, DateTime startDate, DateTime endDate)
    {
        var userObjectId = new ObjectId(userId);

        var filter = Builders<TakenRequirement>.Filter.And(
            Builders<TakenRequirement>.Filter.Eq(x => x.User, userObjectId),
            Builders<TakenRequirement>.Filter.Gte(x => x.Date, startDate),
            Builders<TakenRequirement>.Filter.Lte(x => x.Date, endDate)
        );

        return await Collection.Find(filter).ToListAsync();
    }

    public async Task<List<TakenRequirement>> GetByDate(DateTime startDate, DateTime endDate)
    {
        var filter = Builders<TakenRequirement>.Filter.And(
            Builders<TakenRequirement>.Filter.Gte(x => x.Date, startDate),
            Builders<TakenRequirement>.Filter.Lte(x => x.Date, endDate)
        );
        return await Collection.Find(filter).ToListAsync();
    }
}