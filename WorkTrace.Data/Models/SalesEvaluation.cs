using MongoDB.Bson;
using WorkTrace.Data.Common.Generics;

namespace WorkTrace.Data.Models;

public class SalesEvaluation : BaseModel
{
    public ObjectId TargetUserId { get; set; }
    public ObjectId EvaluatorClientId { get; set; } 
    public ObjectId FormTemplateId { get; set; }
    public List<FormAnswers> Answers { get; set; }
    public string ClientComment { get; set; }
    public ClientSignature ClientSignature { get; set; }
    public DateTime CreatedAt { get; set; }
}