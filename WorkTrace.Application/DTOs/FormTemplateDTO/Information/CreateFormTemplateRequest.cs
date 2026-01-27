namespace WorkTrace.Application.DTOs.FormTemplateDTO.Information;

public class CreateFormTemplateRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public List<CreateFormQuestionRequest> Questions { get; set; }
    public bool IsActive { get; set; }
}