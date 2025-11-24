using MongoDB.Bson;
using MongoDB.Driver;
using WorkTrace.Application.Repositories;
using WorkTrace.Data;
using WorkTrace.Data.Models;

namespace WorkTrace.Repositories.Repositories;

public class AssignmentRepository : GenericRepository<Assignment>, IAssignmentRepository
{
    public AssignmentRepository(WorkTraceContext context)
    {
        Collection = context.GetCollection<Assignment>("assignments");
    }

    public async Task<List<BsonDocument>> GetClientAssignmentRawAsync(string clientId)
    {
        var clientObjectId = new ObjectId(clientId);

        var pipeline = Collection.Aggregate()
            .Match(a => a.Client == clientObjectId)
            .SortByDescending(a => a.AssignedDate)
            .Lookup("services", "Service", "_id", "ServiceInfo")
            .Lookup("status", "Status", "_id", "StatusInfo")
            .Lookup("users", "Users", "_id", "UserInfo")
            .Project(new BsonDocument
            {

                { "Service", new BsonDocument("$arrayElemAt", new BsonArray { "$ServiceInfo.Name", 0 }) },
                { "AssignedDate", "$AssignedDate" },
                { "CheckOut", "$CheckOut" },
                { "Status", new BsonDocument("$arrayElemAt", new BsonArray { "$StatusInfo.Name", 0 }) },
                { "Address", "$Address" },
                { "Users", "$UserInfo.FullName" }
            });
        return await pipeline.ToListAsync();
    }

    public async Task<List<Assignment>> GetAssignmentByUserAndDateRangeAsync(string userId, DateTime start, DateTime end)
    {
        var userObjectId = new ObjectId(userId);

        var filter = Builders<Assignment>.Filter.And(
            Builders<Assignment>.Filter.AnyEq(a => a.Users, userObjectId),
            Builders<Assignment>.Filter.Gte(a => a.AssignedDate, start),
            Builders<Assignment>.Filter.Lte(a => a.AssignedDate, end)
        );
        return await Collection.Find(filter).ToListAsync();
    }
}