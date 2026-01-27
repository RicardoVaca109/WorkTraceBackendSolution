using FluentValidation;

namespace WorkTrace.Application.DTOs.FormTemplateDTO.Information;

internal class CreateFormQuestionRequestValidator : AbstractValidator<CreateFormQuestionRequest>
{
    public CreateFormQuestionRequestValidator()
    {
        RuleFor(x => x.QuestionKey)
            .NotEmpty()
            .MaximumLength(64);

        RuleFor(x => x.QuestionText)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.AnswerType)
            .Must(value => Enum.IsDefined(typeof(Enums.AnswerType), value))
            .WithMessage("'Tipo de Respuesta' debe ser un valor válido");
    }
}