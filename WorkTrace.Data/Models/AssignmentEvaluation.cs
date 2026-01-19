using MongoDB.Bson;
using WorkTrace.Data.Common.Generics;

namespace WorkTrace.Data.Models;

public class AssignmentEvaluation : BaseModel
{
    public ObjectId AssignmentId { get; set; }
    public ObjectId FormTemplateId { get; set; }
    public List<UserEvaluation> UserEvaluations { get; set; }
    public string ClientComment { get; set; }
    public ClientSignature ClientSignature { get; set; }
    public DateTime CreatedAt { get; set; }
}