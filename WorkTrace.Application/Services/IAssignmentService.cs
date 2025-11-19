using WorkTrace.Application.DTOs.AssignmentDTO.Management;

namespace WorkTrace.Application.Services;

public interface IAssignmentService
{
    Task<AssignmentResponse> CreateAssigmentAdminAsync(CreateAssignmentRequest assignmentRequest);
    Task<List<AssignmentResponse>> GetAllAsync();
    Task<AssignmentResponse> GetByIdAsync(string id);
    Task<List<ClientHistoryResponse>> GetClientHistoryAsync(string cliendId);
    Task ValidateExistance(CreateAssignmentRequest assignmentRequest);
}
