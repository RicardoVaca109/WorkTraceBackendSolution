using MongoDB.Bson;

namespace WorkTrace.Data.Models
{
    public class Assignment
    {
        public ObjectId Id { get; set; }
        public List<ObjectId> Users { get; set; }
        public ObjectId Service { get; set; }
        public ObjectId Client { get; set; }
        public ObjectId Status { get; set; }
        public DateTime Date { get; set; }
        public string Address { get; set; }

    }
}
