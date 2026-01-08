namespace WorkTrace.Application.DTOs.FormTemplateDTO.Information;

public class UpdateFormTemplateRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }
    public List<UpdateFormQuestionsRequest> Questions { get; set; }
}