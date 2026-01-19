using FluentValidation;

namespace WorkTrace.Application.DTOs.AssignmentEvaluationDTO;

public class CreateAssignmentEvaluationValidator : AbstractValidator<CreateAssignmentEvaluationRequest>
{
    public CreateAssignmentEvaluationValidator()
    {
        RuleFor(x => x.AssignmentId)
            .NotEmpty()
            .Must(BeValidObjectId)
            .WithMessage("El AssignmentId no es válido");

        RuleFor(x => x.FormType)
            .NotEmpty()
            .WithMessage("Debe indicar el tipo de formulario");

        RuleFor(x => x.UserEvaluations)
            .NotNull()
            .Must(x => x.Any())
            .WithMessage("Debe existir al menos una evaluación");

        RuleForEach(x => x.UserEvaluations)
            .SetValidator(new UserEvaluationValidator());

        RuleFor(x => x.ClientSignature)
            .NotNull()
            .SetValidator(new ClientSignatureValidator());
    }

    private bool BeValidObjectId(string id)
    {
        return MongoDB.Bson.ObjectId.TryParse(id, out _);
    }
}