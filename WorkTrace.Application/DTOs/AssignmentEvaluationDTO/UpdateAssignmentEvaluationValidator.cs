using FluentValidation;

namespace WorkTrace.Application.DTOs.AssignmentEvaluationDTO;

public class UpdateAssignmentEvaluationValidator : AbstractValidator<UpdateAssignmentEvaluationRequest>
{
    public UpdateAssignmentEvaluationValidator()
    {
        RuleFor(x => x.UserEvaluations)
            .NotEmpty()
            .WithMessage("Debe existir al menos una evaluación");

        RuleForEach(x => x.UserEvaluations)
            .SetValidator(new UserEvaluationValidator());
    }
}