using MongoDB.Bson;

namespace WorkTrace.Data.Models
{
    public class InstallationStep
    {
        public ObjectId Id { get; set; }
        public int Steps { get; set; }
        public string Description { get; set; }
    }
}
