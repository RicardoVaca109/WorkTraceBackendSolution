using MongoDB.Bson;

namespace WorkTrace.Data.Models
{
    public class Client
    {
        public ObjectId Id { get; set; }
        public string FullName { get; set; }
        public string DocumentNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
    }
}
