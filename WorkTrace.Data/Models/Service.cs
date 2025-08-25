using MongoDB.Bson;

namespace WorkTrace.Data.Models
{
    public class Service
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<ObjectId> InstallationSteps { get; set; }
    }
}
