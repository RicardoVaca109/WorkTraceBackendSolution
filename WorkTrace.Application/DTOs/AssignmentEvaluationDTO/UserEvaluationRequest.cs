namespace WorkTrace.Application.DTOs.AssignmentEvaluationDTO;

public class UserEvaluationRequest
{
    public string UserId { get; set; }
    public List<FormAnswerRequest> Answers { get; set; }
}