using FluentValidation;
using Microsoft.AspNetCore.Http;

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
            .WithMessage("La firma del cliente es obligatoria")
            .Must(BeValidImage)
            .WithMessage("La firma debe ser una imagen PNG o JPG")
            .Must(BeValidSize)
            .WithMessage("La firma no puede superar 2MB");

        RuleFor(x => x.SignedBy)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("El nombre del firmante es obligatorio");
    }

    private bool BeValidObjectId(string id)
    {
        return MongoDB.Bson.ObjectId.TryParse(id, out _);
    }

    private bool BeValidImage(IFormFile file)
    {
        if (file == null) return false;

        return file.ContentType == "image/png"
            || file.ContentType == "image/jpeg";
    }
    private bool BeValidSize(IFormFile file)
    {
        if (file == null) return false;

        const long maxSize = 2 * 1024 * 1024; // 2MB
        return file.Length <= maxSize;
    }
}