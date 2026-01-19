using FluentValidation;
using WorkTrace.Application.Enums;

namespace WorkTrace.Application.DTOs.AssignmentEvaluationDTO;

public class FormAnswerValidator : AbstractValidator<FormAnswerRequest>
{
    public FormAnswerValidator()
    {
        RuleFor(x => x.QuestionKey)
            .NotEmpty();

        RuleFor(x => x.QuestionValue)
            .NotEmpty();

        RuleFor(x => x.AnswerType)
            .IsInEnum();

        When(x => x.AnswerType == AnswerType.Text, () =>
        {
            RuleFor(x => x.Answer)
                .NotEmpty()
                .WithMessage("La respuesta es obligatoria");
        });

        When(x => x.AnswerType == AnswerType.Numeric, () =>
        {
            RuleFor(x => x.NumericValue)
                .NotNull()
                .InclusiveBetween(0, 5);
        });
    }
}