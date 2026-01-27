using WorkTrace.Application.Enums;

namespace WorkTrace.Application.DTOs.FormTemplateDTO.Information;

public class CreateFormQuestionRequest
{
    public string QuestionKey { get; set; }
    public string QuestionText { get; set; }
    public AnswerType AnswerType { get; set; }
}