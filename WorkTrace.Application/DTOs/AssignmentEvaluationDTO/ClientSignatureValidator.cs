using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace WorkTrace.Application.DTOs.AssignmentEvaluationDTO;

public class ClientSignatureValidator :  AbstractValidator<ClientSignatureRequest>
{
    public ClientSignatureValidator()
    {
        RuleFor(x => x.SignatureFile)
            .NotNull()
            .WithMessage("La firma del cliente es obligatoria")
            .Must(BeValidImage)
            .WithMessage("La firma debe ser una imagen válida (PNG o JPG)")
            .Must(BeValidSize)
            .WithMessage("La firma no puede superar 2MB");

        RuleFor(x => x.SignedBy)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("El nombre del firmante es obligatorio");
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
