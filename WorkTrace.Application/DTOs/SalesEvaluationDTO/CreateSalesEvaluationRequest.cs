using Microsoft.AspNetCore.Http;
using WorkTrace.Application.DTOs.AssignmentEvaluationDTO;

namespace WorkTrace.Application.DTOs.SalesEvaluationDTO;

public class CreateSalesEvaluationRequest
{
    public string TargetUserId { get; set; }
    public string FormType { get; set; }
    public List<CreateFormAnswerRequest> Answers { get; set; }
    public string ClientComment { get; set; }
    public IFormFile ClientSignature { get; set; }
    public string SignedBy { get; set; }
}