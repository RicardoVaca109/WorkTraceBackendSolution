namespace WorkTrace.Application.DTOs.AssignmentEvaluationDTO;

public class ClientSignatureRequest
{
    public string SignatureBase64 { get; set; }
    public string SignedBy { get; set; }
}