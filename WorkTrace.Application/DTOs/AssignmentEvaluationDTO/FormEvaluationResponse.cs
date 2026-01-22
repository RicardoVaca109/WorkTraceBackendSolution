namespace WorkTrace.Application.DTOs.AssignmentEvaluationDTO;

public class FormEvaluationResponse
{
    public string FormTemplateId { get; set; }
    public string FormName { get; set; }
    public List<FormEvaluationQuestionResponse> Questions { get; set; } = new();
    public List<UserEvaluationResponse> UserEvaluations { get; set; } = new();
}