using FluentValidation;

namespace WorkTrace.Application.DTOs.FormTemplateDTO.Information;

public class UpdateFormTemplateRequestValidator : AbstractValidator<UpdateFormTemplateRequest>
{
    public UpdateFormTemplateRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(126);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.Questions)
            .NotNull()
            .Must(q => q.Count > 0)
            .WithMessage("Debe existir al menos una pregunta.");

        RuleForEach(x => x.Questions)
            .SetValidator(new UpdateFormQuestionsRequestValidator());

        RuleFor(x => x.Questions)
            .Must(HaveUniqueKeys)
            .WithMessage("No pueden existir QuestionKey duplicadas.");
    }

    private bool HaveUniqueKeys(List<UpdateFormQuestionsRequest> questions)
    {
        return questions
            .Select(q => q.QuestionKey)
            .Distinct()
            .Count() == questions.Count;
    }
}