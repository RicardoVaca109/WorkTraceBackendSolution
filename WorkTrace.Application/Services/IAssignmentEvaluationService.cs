using WorkTrace.Application.DTOs.AssignmentEvaluationDTO;

namespace WorkTrace.Application.Services;

public interface IAssignmentEvaluationService
{
    Task<AssignmentEvaluationResponse> CreateEvaluationAsync(CreateAssignmentEvaluationRequest request);
    Task<AssignmentEvaluationResponse?> GetActiveByAssignmentAsync(string assignmentId);
    Task<List<AssignmentEvaluationResponse>> GetHistoryByAssignmentAsync(string assignmentId);
    Task UpdateEvaluationAsync(string evaluationId, UpdateAssignmentEvaluationRequest request);
    Task<AssignmentEvaluationDetailResponse> GetEvaluationDetailByAssignmentAsync(string assignmentId);
}