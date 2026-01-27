namespace WorkTrace.Application.DTOs.AssignmentEvaluationDTO;

public class AssignmentEvaluationResponse
{
    public string Id { get; set; }
    public string AssignmentId { get; set; }
    public string FormType { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}