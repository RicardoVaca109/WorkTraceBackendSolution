using FluentValidation;

namespace WorkTrace.Application.DTOs.ServiceMgmtDTO.Management;

public class InstallationStepValidator : AbstractValidator<CreateInstallationStepRequest>
{
    public InstallationStepValidator()
    {
        RuleFor(x => x.Steps)
            .NotEmpty()
            .GreaterThan(0).WithMessage("Step number must be positive greater than > 0");
        RuleFor(x => x.Description)
            .NotEmpty()
            .Length(1, 256);
    }
}