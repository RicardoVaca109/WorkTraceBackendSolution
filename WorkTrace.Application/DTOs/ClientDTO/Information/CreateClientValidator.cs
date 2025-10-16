using FluentValidation;

namespace WorkTrace.Application.DTOs.ClientDTO.Information;

public class CreateClientValidator : AbstractValidator<CreateClientRequest>
{
    public CreateClientValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .Length(1, 64);
        RuleFor(x => x.DocumentNumber)
            .NotEmpty()
            .Length(10, 16);
        RuleFor(X => X.PhoneNumber)
            .NotEmpty()
            .Length(10, 14);
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .Length(6, 128);
    }
}