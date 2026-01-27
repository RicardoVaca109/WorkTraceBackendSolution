namespace WorkTrace.Application.DTOs.AssignmentEvaluationDTO;

public class UserEvaluationResponse
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public int Score { get; set; }
    public List<FormAnswerResponse> Answers { get; set; } = new();
}