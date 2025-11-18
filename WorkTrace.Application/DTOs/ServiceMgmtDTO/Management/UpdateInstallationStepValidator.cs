using FluentValidation;

namespace WorkTrace.Application.DTOs.ServiceMgmtDTO.Management;

public class UpdateInstallationStepValidator : AbstractValidator<UpdateInstallationStepRequest>
{
    public UpdateInstallationStepValidator()
    {
        RuleFor(x => x.Steps)
            .NotEmpty()
            .GreaterThan(0)
            .WithMessage("Step number must be greater than 0");

        RuleFor(x => x.Description)
            .NotEmpty()
            .Length(1, 256);
    }
}