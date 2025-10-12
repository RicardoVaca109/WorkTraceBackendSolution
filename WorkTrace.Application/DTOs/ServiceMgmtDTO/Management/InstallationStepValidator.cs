using FluentValidation;

namespace WorkTrace.Application.DTOs.ServiceMgmtDTO.Management;

public class InstallationStepValidator : AbstractValidator<CreateInstallationStepRequest>
{
    public InstallationStepValidator()
    {
        RuleFor(x => x.Steps)
            .GreaterThan(0).WithMessage("Step number must be positive.");
        RuleFor(x => x.Description)
            .NotEmpty()
            .Length(1, 256);
    }
}