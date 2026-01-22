namespace WorkTrace.Application.DTOs.AssignmentEvaluationDTO;

public class FormAnswerResponse
{
    public string QuestionKey { get; set; }
    public string QuestionValue { get; set; }
    public string? Answer { get; set; }
    public int? NumericValue { get; set; }
}