using MongoDB.Bson;

namespace WorkTrace.Data.Models
{
    public class TakenRequirement
    {
        public ObjectId Id { get; set; }
        public ObjectId User { get; set; }
        public ObjectId Client {  get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }
}
