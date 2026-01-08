using FluentValidation;

namespace WorkTrace.Application.DTOs.FormTemplateDTO.Information;

public class CreateFormTemplateRequestValidator : AbstractValidator<CreateFormTemplateRequest>
{
    public CreateFormTemplateRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(300);

        RuleFor(x => x.Questions)
            .NotNull()
            .Must(q => q.Count > 0)
            .WithMessage("El formulario debe tener al menos una pregunta.");

        RuleForEach(x => x.Questions)
            .SetValidator(new CreateFormQuestionRequestValidator());

        RuleFor(x => x.Questions)
            .Must(HaveUniqueKeys)
            .WithMessage("No pueden existir QuestionKey duplicadas.");
    }

    private bool HaveUniqueKeys(List<CreateFormQuestionRequest> questions)
    {
        return questions
            .Select(q => q.QuestionKey)
            .Distinct()
            .Count() == questions.Count;
    }
}