using WorkTrace.Application.DTOs.AssignmentEvaluationDTO;
using WorkTrace.Application.DTOs.ServiceMgmtDTO.Management;

namespace WorkTrace.Application.DTOs.AssignmentDTO.Mobile;

public class StartAssignmentDetailResponse
{
    public string AssignmentId { get; set; }
    public string ServiceName { get; set; }
    public string ServiceDescription { get; set; }
    public List<InstallationStepResponse> InstallationSteps { get; set; } = new();
}