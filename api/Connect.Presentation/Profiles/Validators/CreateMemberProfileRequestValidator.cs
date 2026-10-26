using Connect.Domain.Users;
using Connect.Presentation.Profiles.DTO;
using FluentValidation;

namespace Connect.Presentation.Profiles.Validators;

public sealed class CreateMemberProfileRequestValidator
    : AbstractValidator<CreateMemberProfileRequest>
{
    private static readonly string[] AgeBandNames = Enum.GetNames<AgeBand>();

    public CreateMemberProfileRequestValidator()
    {
        RuleFor(request => request.FirstName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Fill in your first name")
            .MaximumLength(60)
            .WithMessage("Use at most 60 characters for your first name");

        RuleFor(request => request.AgeBand)
            .Must(ageBand => ageBand is not null && AgeBandNames.Contains(ageBand))
            .WithMessage("Choose your age band");

        RuleFor(request => request.Region)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Fill in your town or area")
            .MaximumLength(60)
            .WithMessage("Use at most 60 characters for your town or area");

        RuleFor(request => request.Bio)
            .MaximumLength(280)
            .WithMessage("Your short line can be at most 280 characters");
    }
}
