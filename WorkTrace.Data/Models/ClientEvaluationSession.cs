using MongoDB.Bson;
using WorkTrace.Data.Common.Generics;

namespace WorkTrace.Data.Models;

public class ClientEvaluationSession : BaseModel
{
    public ObjectId AssignmentId { get; set; }
    public ObjectId ClientId { get; set; }
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsAuthenticated { get; set; }
    public bool IsUsed { get; set; }
}