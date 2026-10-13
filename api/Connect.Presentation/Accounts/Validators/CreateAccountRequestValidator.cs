using Connect.Presentation.Accounts.DTO;
using FluentValidation;

namespace Connect.Presentation.Accounts.Validators;

public sealed class CreateAccountRequestValidator
    : AbstractValidator<CreateAccountRequest>
{
    public CreateAccountRequestValidator()
    {
        RuleFor(request => request.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Fill in a valid email address")
            .EmailAddress()
            .WithMessage("Fill in a valid email address");

        RuleFor(request => request.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Fill in a password")
            .MinimumLength(15)
            .WithMessage("Password must be at least 15 characters")
            .Must(password => password.Any(char.IsDigit))
            .WithMessage("Password must contain at least one digit")
            .Must(password => password.Any(char.IsLower))
            .WithMessage("Password must contain at least one lowercase letter")
            .Must(password => password.Any(char.IsUpper))
            .WithMessage("Password must contain at least one uppercase letter")
            .Must(password => password.Any(character => !char.IsLetterOrDigit(character)))
            .WithMessage("Password must contain at least one non-alphanumeric character");
    }
}
