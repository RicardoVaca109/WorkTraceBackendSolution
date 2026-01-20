using WorkTrace.Application.DTOs.FormTemplateDTO.Information;

namespace WorkTrace.Application.DTOs.AssignmentEvaluationDTO;

public class TechnicianEvaluationFormResponse
{
    public string TechnicianId { get; set; }
    public string TechnicianName { get; set; }

    public string FormTemplateId { get; set; }
    public string FormName { get; set; }
    public bool IsCompleted { get; set; }
    public List<FormQuestionResponse> Questions { get; set; }
}