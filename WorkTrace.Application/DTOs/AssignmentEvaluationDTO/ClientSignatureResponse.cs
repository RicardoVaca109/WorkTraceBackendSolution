namespace WorkTrace.Application.DTOs.AssignmentEvaluationDTO;

public class ClientSignatureResponse
{
    public string Url { get; set; }
    public string SignedBy { get; set; }
    public DateTime SignedAt { get; set; }
}