namespace WorkTrace.Application.DTOs.FormTemplateDTO.Information;

public class FormTemplateResponse
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<FormQuestionResponse> Questions { get; set; }
}