using FluentValidation;
using WorkTrace.Application.Enums;

namespace WorkTrace.Application.DTOs.FormTemplateDTO.Information;

public class UpdateFormQuestionsRequestValidator : AbstractValidator<UpdateFormQuestionsRequest>
{
    public UpdateFormQuestionsRequestValidator()
    {
        RuleFor(x => x.QuestionKey)
            .NotEmpty()
            .MaximumLength(64);

        RuleFor(x => x.QuestionText)
            .NotEmpty()
            .MaximumLength(264);

        RuleFor(x => x.AnswerType)
            .Must(value => Enum.IsDefined(typeof(AnswerType), value))
            .WithMessage("'Tipo de Respuesta' debe ser un valor válido");
    }
}