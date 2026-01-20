using Microsoft.AspNetCore.Http;
using WorkTrace.Data.Models;

namespace WorkTrace.Application.DTOs.AssignmentEvaluationDTO;

public class ClientSignatureRequest
{
    public IFormFile SignatureFile { get; set; }
    public string SignedBy { get; set; }
}