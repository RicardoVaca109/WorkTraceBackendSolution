using FluentValidation;

namespace WorkTrace.Application.DTOs.AssignmentDTO.Management;

public class CreateAssignmentValidator : AbstractValidator<CreateAssignmentRequest>
{
    public CreateAssignmentValidator()
    {
        RuleFor(x => x.Users)
            .NotEmpty().WithMessage("Users list cannot be empty.");

        RuleForEach(x => x.Users)
            .NotEmpty().WithMessage("User Id cannot be empty.")
            .Must(BeValidObjectId).WithMessage("User Id must be a valid ObjectId.");

        RuleFor(x => x.Service)
            .NotEmpty().WithMessage("Service Id is required.")
            .Must(BeValidObjectId).WithMessage("Service Id must be a valid ObjectId.");

        RuleFor(x => x.Client)
            .NotEmpty().WithMessage("Client Id is required.")
            .Must(BeValidObjectId).WithMessage("Client Id must be a valid ObjectId.");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status Id is required.")
            .Must(BeValidObjectId).WithMessage("Status Id must be a valid ObjectId.");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required.")
            .MinimumLength(5).WithMessage("Address must have at least 5 characters.");

        RuleFor(x => x.AssignedDate)
            .NotEmpty().WithMessage("Date is required.")
            .GreaterThan(DateTime.MinValue);

        RuleFor(x => x.CreatedByUser)
            .NotEmpty().WithMessage("CreatedByUser is required.")
            .Must(BeValidObjectId).WithMessage("CreatedByUser must be a valid ObjectId.");
    }
    private bool BeValidObjectId(string id)
    {
        return MongoDB.Bson.ObjectId.TryParse(id, out _);
    }
}
