using Connect.Domain.Tags;
using Connect.Presentation.Profiles.DTO;
using FluentValidation;

namespace Connect.Presentation.Profiles.Validators;

// One validator for every address that adds a tag. The endpoint normalises the name first.
public sealed class AddTagRequestValidator : AbstractValidator<AddTagRequest>
{
    public AddTagRequestValidator()
    {
        RuleFor(request => request.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Fill in a tag")
            .MaximumLength(TagRules.MaxLength)
            .WithMessage("A tag has at most 30 characters");
    }
}
