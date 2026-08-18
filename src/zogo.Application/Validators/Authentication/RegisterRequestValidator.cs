using FluentValidation;
using zogo.Application.DTOs.Authentication;

namespace zogo.Application.Validators.Authentication;

public sealed class RegisterRequestValidator
    : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(255);

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(30)
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(100)
            .Matches("[A-Z]")
            .WithMessage(
                "Password must contain at least one uppercase letter.")
            .Matches("[a-z]")
            .WithMessage(
                "Password must contain at least one lowercase letter.")
            .Matches("[0-9]")
            .WithMessage(
                "Password must contain at least one number.");
    }
}