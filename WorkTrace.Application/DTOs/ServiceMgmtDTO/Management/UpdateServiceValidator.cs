using FluentValidation;


namespace WorkTrace.Application.DTOs.ServiceMgmtDTO.Management;

public class UpdateServiceValidator : AbstractValidator<UpdateServiceRequest>
{
    public UpdateServiceValidator()
    {
        RuleFor(x => x.Name)
              .NotEmpty()
              .Length(1, 128);

        RuleFor(x => x.Description)
            .NotEmpty()
            .Length(1, 256);

        RuleFor(x => x.InstallationSteps)
            .NotNull().WithMessage("Steps list cannot be null.")
            .NotEmpty().WithMessage("At least one step is required.")
            .Must(steps => steps == null || steps.Select(s => s.Steps).Distinct().Count() == steps.Count)
            .WithMessage("Step numbers must be unique.");

        RuleForEach(x => x.InstallationSteps)
            .SetValidator(new UpdateInstallationStepValidator());
    }
}
