using FluentValidation;

namespace WorkTrace.Application.DTOs.AssignmentDTO.Management;

public class UpdateAssignmentWebValidator : AbstractValidator<UpdateAssignmentWebRequest>
{
    public UpdateAssignmentWebValidator()
    {

        // Validar lista de usuarios si se envía
        RuleForEach(x => x.Users)
            .Must(BeValidObjectId).WithMessage("User Id must be a valid ObjectId.");

        // Validar Service si se envía
        RuleFor(x => x.Service)
            .Must(BeValidObjectId).When(x => !string.IsNullOrEmpty(x.Service))
            .WithMessage("Service Id must be a valid ObjectId.");

        // Validar Client si se envía
        RuleFor(x => x.Client)
            .Must(BeValidObjectId).When(x => !string.IsNullOrEmpty(x.Client))
            .WithMessage("Client Id must be a valid ObjectId.");

        // Validar Status si se envía
        RuleFor(x => x.Status)
            .Must(BeValidObjectId).When(x => !string.IsNullOrEmpty(x.Status))
            .WithMessage("Status Id must be a valid ObjectId.");

        // Validar Address si se envía
        RuleFor(x => x.Address)
            .MinimumLength(5).When(x => !string.IsNullOrEmpty(x.Address))
            .WithMessage("Address must have at least 5 characters.");

        // Validar AssignedDate si se envía
        RuleFor(x => x.AssignedDate)
            .GreaterThan(DateTime.MinValue).When(x => x.AssignedDate.HasValue)
            .WithMessage("AssignedDate must be a valid date.");

        // Validar CreatedByUser si se envía
        RuleFor(x => x.CreatedByUser)
            .Must(BeValidObjectId).When(x => !string.IsNullOrEmpty(x.CreatedByUser))
            .WithMessage("CreatedByUser must be a valid ObjectId.");
    }

    private bool BeValidObjectId(string id)
    {
        return MongoDB.Bson.ObjectId.TryParse(id, out _);
    }

}
