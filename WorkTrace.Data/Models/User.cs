using MongoDB.Bson;

namespace WorkTrace.Data.Models
{
    public class User
    {
        public ObjectId Id { get; set; }
        public List<ObjectId> Services { get; set; }
        public string FullName { get; set; }
        public string DocumentNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
    }
}
