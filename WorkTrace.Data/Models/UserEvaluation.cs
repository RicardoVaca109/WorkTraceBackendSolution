using MongoDB.Bson;

namespace WorkTrace.Data.Models;

public class UserEvaluation
{
    public ObjectId UserId { get; set; }
    public int Score { get; set; }
    public List<FormAnswers> Answers { get; set; }
}