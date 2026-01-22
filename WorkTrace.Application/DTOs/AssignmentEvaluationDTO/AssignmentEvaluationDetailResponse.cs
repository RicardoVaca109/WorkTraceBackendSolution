namespace WorkTrace.Application.DTOs.AssignmentEvaluationDTO;

public class AssignmentEvaluationDetailResponse
{
    public string AssignmentId { get; set; }

    public string? ClientComment { get; set; }

    public ClientSignatureResponse? ClientSignature { get; set; }

    public List<MediaFileResponse> MediaFiles { get; set; } = new();

    public List<FormEvaluationResponse> Forms { get; set; } = new();
}