using FluentValidation;

namespace WorkTrace.Application.DTOs.AssignmentEvaluationDTO;

public class UserEvaluationValidator : AbstractValidator<UserEvaluationRequest>
{
    public UserEvaluationValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("El usuario a evaluar es obligatorio")
            .Must(BeValidObjectId)
            .WithMessage("El Id del usuario no es válido");

        RuleFor(x => x.Answers)
            .NotNull()
            .WithMessage("Debe existir un conjunto de respuestas")
            .Must(a => a.Any())
            .WithMessage("Debe existir al menos una respuesta");

        RuleForEach(x => x.Answers)
            .SetValidator(new FormAnswerValidator());
    }

    private bool BeValidObjectId(string id)
    {
        return MongoDB.Bson.ObjectId.TryParse(id, out _);
    }
}