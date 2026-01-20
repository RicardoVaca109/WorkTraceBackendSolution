using WorkTrace.Application.DTOs.SalesEvaluationDTO;

namespace WorkTrace.Application.Services;

public interface ISalesEvaluationService
{
    Task SubmitEvaluationAsync(CreateSalesEvaluationRequest request, string evaluatorClientId);
}