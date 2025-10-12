using FluentValidation;

namespace WorkTrace.Application.DTOs.ServiceMgmtDTO.Management;

public class ServiceValidator : AbstractValidator<CreateServiceRequest>
{
    public ServiceValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .Length(1,128);
        RuleFor(x => x.Description)
            .NotEmpty()
            .Length(1,256);
        RuleFor(x => x.InstallationSteps)
            .NotNull()
            .NotEmpty().WithMessage("At least one step is required.");
        RuleForEach(x => x.InstallationSteps)
            .SetValidator(new InstallationStepValidator());
    }
}