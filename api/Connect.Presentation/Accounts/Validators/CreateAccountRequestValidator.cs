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
            .NotEmpty()
            .MinimumLength(15)
            .WithMessage("Password must be at least 15 characters")
            .Matches("[0-9]")
            .WithMessage("Password must contain at least one digit")
            .Matches("[a-z]")
            .WithMessage("Password must contain at least one lowercase letter")
            .Matches("[A-Z]")
            .WithMessage("Password must contain at least one uppercase letter")
            .Matches("[^a-zA-Z0-9]")
            .WithMessage("Password must contain at least one non-alphanumeric character");
    }
}
