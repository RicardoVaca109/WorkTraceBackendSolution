using FluentValidation;
using MongoDB.Bson;

namespace WorkTrace.Application.DTOs.TakenRequirementDTO;

public class UpdateTakenRequirementRequestValidator : AbstractValidator<UpdateTakenRequirementRequest>
{
    public UpdateTakenRequirementRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .Must(BeValidObjectId)
            .WithMessage("Id no es un ObjectId válido");

        RuleFor(x => x.ClientId)
            .Must(id => string.IsNullOrWhiteSpace(id) || BeValidObjectId(id))
            .WithMessage("ClientId no es un ObjectId válido");

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(128);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(512);
    }

    private bool BeValidObjectId(string id)
    {
        return ObjectId.TryParse(id, out _);
    }
}