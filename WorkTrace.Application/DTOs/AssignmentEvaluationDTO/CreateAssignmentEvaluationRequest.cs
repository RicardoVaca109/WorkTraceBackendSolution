namespace WorkTrace.Application.DTOs.AssignmentEvaluationDTO;

public class CreateAssignmentEvaluationRequest
{
    public string AssignmentId { get; set; }
    public string FormType { get; set; }

    public List<UserEvaluationRequest> UserEvaluations { get; set; }

    public string ClientComment { get; set; }

    public ClientSignatureRequest ClientSignature { get; set; }
}