using WorkTrace.Application.DTOs.AssignmentEvaluationDTO;
using WorkTrace.Application.DTOs.ClientSessionDTO;
using WorkTrace.Data.Models;

namespace WorkTrace.Application.Services;

public interface IClientEvaluationService
{
    Task<CreateEvaluationSessionResponse> CreateSessionAsync(string assignmentId);
    Task<ClientEvaluationLoginResponse> LoginAsync(ClientEvaluationLoginRequest request);
    Task SubmitEvaluationAsync(CreateAssignmentEvaluationRequest request, ClientEvaluationSession session);
    Task<AssignmentEvaluationFormResponse> GetEvaluationFormAsync(string assignmentId);
}