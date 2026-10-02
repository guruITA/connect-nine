using FluentValidation;

namespace Connect.Presentation.Authentication;

public sealed class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(request => request.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("A valid email address is required.")
            .EmailAddress()
            .WithMessage("A valid email address is required.")
            .OverridePropertyName("email");

        RuleFor(request => request.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(8)
            .WithMessage("Password must contain at least 8 characters.")
            .OverridePropertyName("password");

        RuleFor(request => request.DisplayName)
            .NotEmpty()
            .WithMessage("Display name is required.")
            .OverridePropertyName("displayName");
    }
}
