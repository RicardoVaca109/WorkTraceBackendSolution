using FluentValidation;

namespace WorkTrace.Application.DTOs.AssignmentEvaluationDTO;

public class ClientSignatureValidator :  AbstractValidator<ClientSignatureRequest>
{
    public ClientSignatureValidator()
    {
        RuleFor(x => x.SignatureBase64)
            .NotEmpty()
            .WithMessage("La firma del cliente es obligatoria");

        RuleFor(x => x.SignedBy)
            .NotEmpty()
            .Must(BeValidObjectId)
            .WithMessage("El usuario que firma debe ser válido");
    }

    private bool BeValidObjectId(string id)
    {
        return MongoDB.Bson.ObjectId.TryParse(id, out _);
    }
}
