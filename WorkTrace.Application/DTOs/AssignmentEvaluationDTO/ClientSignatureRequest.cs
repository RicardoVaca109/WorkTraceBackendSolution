using Microsoft.AspNetCore.Http;

namespace WorkTrace.Application.DTOs.AssignmentEvaluationDTO;

public class ClientSignatureRequest
{
    public IFormFile SignatureFile { get; set; }
    public string SignedBy { get; set; }
}