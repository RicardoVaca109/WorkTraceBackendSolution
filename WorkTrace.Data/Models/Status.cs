using MongoDB.Bson;

namespace WorkTrace.Data.Models
{
    public class Status
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
