using FluentValidation;

namespace WorkTrace.Application.DTOs.AssignmentDTO.Management;

public class UpdateAssignmentMobileValidator : AbstractValidator<UpdateAssignmentMobileRequest>
{
    public UpdateAssignmentMobileValidator()
    {
        RuleFor(x => x.CheckOut)
            .Must((request, checkOut) => !checkOut.HasValue || request.CheckIn.HasValue)
            .WithMessage("CheckOut cannot be set without CheckIn.");

        RuleFor(x => x.CurrentLocation)
            .Must(loc => loc.Latitude != 0 && loc.Longitude != 0)
            .When(x => x.CurrentLocation != null)
            .WithMessage("CurrentLocation must have valid coordinates.");

        RuleForEach(x => x.StepsProgress)
            .Must(step => step.CompletedAt == null || step.IsCompleted)
            .WithMessage("CompletedAt can only be set if IsCompleted is true.");

        RuleForEach(x => x.MediaFiles)
            .Must(file => !string.IsNullOrEmpty(file.Url))
            .WithMessage("MediaFile must have a valid URL or Base64 string.");
    }
}