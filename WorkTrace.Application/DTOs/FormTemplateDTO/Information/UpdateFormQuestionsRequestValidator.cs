using FluentValidation;
using WorkTrace.Application.Enums;

namespace WorkTrace.Application.DTOs.FormTemplateDTO.Information;

public class UpdateFormQuestionsRequestValidator : AbstractValidator<UpdateFormQuestionsRequest>
{
    public UpdateFormQuestionsRequestValidator()
    {
        RuleFor(x => x.QuestionKey)
            .MaximumLength(64);

        RuleFor(x => x.QuestionText)
            .MaximumLength(264);

        RuleFor(x => x.AnswerType)
            .Must(value => Enum.IsDefined(typeof(AnswerType), value))
            .When(x => x.AnswerType.HasValue)
            .WithMessage("'Tipo de Respuesta' debe ser un valor válido");
    }
}