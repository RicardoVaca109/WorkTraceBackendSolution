using WorkTrace.Application.Enums;

namespace WorkTrace.Application.DTOs.FormTemplateDTO.Information;

public class UpdateFormQuestionsRequest
{
    public string? Id { get; set; }
    public string QuestionKey { get; set; }
    public string QuestionText { get; set; }
    public AnswerType AnswerType { get; set; }
}