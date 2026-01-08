using MongoDB.Bson;
using WorkTrace.Application.Enums;

namespace WorkTrace.Data.Models;

public class FormQuestion
{
    public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
    public string QuestionKey { get; set; }
    public string QuestionText { get; set; }
    public AnswerType AnswerType { get; set; }
}