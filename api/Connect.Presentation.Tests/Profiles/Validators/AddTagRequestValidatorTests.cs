using Connect.Domain.Tags;
using Connect.Presentation.Profiles.DTO;
using Connect.Presentation.Profiles.Validators;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace Connect.Presentation.Tests.Profiles.Validators;

public sealed class AddTagRequestValidatorTests
{
    private readonly AddTagRequestValidator validator = new();

    [TestCase("")]
    [TestCase(null)]
    public void EmptyName_ReturnsFillInATag(string? name)
    {
        var result = validator.TestValidate(new AddTagRequest(name));

        result.ShouldHaveValidationErrorFor(request => request.Name)
            .WithErrorMessage("Fill in a tag")
            .Only();
    }

    [Test]
    public void NameOf31Characters_ReturnsTheLengthMessage()
    {
        var result = validator.TestValidate(new AddTagRequest(new string('a', 31)));

        result.ShouldHaveValidationErrorFor(request => request.Name)
            .WithErrorMessage("A tag has at most 30 characters")
            .Only();
    }

    [Test]
    public void NameOf30Characters_IsAccepted()
    {
        var result = validator.TestValidate(new AddTagRequest(new string('a', 30)));

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void NameWithSpaces_IsAcceptedOnceNormalised()
    {
        var result = validator.TestValidate(new AddTagRequest(TagRules.Normalize("  Drone   Video ")));

        result.ShouldNotHaveAnyValidationErrors();
    }
}
