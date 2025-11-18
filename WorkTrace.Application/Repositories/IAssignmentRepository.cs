using MongoDB.Bson;
using WorkTrace.Data.Models;

namespace WorkTrace.Application.Repositories;

public interface IAssignmentRepository : IGenericRepository<Assignment>
{
    Task<List<BsonDocument>> GetClientAssignmentRawAsync(string clientId);
}