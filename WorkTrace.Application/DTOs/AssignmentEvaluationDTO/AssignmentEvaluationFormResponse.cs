namespace WorkTrace.Application.DTOs.AssignmentEvaluationDTO;

public class AssignmentEvaluationFormResponse
{
    public string AssignmentId { get; set; }
    public string ClientId { get; set; }
    public List<TechnicianEvaluationFormResponse> Evaluations { get; set; }
}