using Microsoft.Extensions.Options;
using MongoDB.Driver;
using WorkTrace.Data.Common.Setttings;

namespace WorkTrace.Data
{
    public class WorkTraceContext
    {
        private readonly MongoClient _mongoClient;
        private readonly IMongoDatabase _database;
        public WorkTraceContext(IOptions<WorkTraceDatabaseSettings> workTraceDatabaseSettings) 
        {
            _mongoClient = new MongoClient(workTraceDatabaseSettings.Value.ConnectionString);

           _database = _mongoClient.GetDatabase(workTraceDatabaseSettings.Value.DataBaseName);
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName) 
        {
            return _database.GetCollection<T>(collectionName);  
        }
    }
}
