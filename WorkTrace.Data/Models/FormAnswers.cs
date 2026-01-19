using WorkTrace.Application.Enums;

namespace WorkTrace.Data.Models;

public class FormAnswers
{
    public string QuestionKey { get; set; }
    public string QuestionValue { get; set; }
    public AnswerType AnswerType { get; set; }
    public string? Answer { get; set; }
    public int? NumericValue { get; set; }
}