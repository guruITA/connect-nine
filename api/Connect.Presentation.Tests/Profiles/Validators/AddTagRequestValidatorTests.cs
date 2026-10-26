using Connect.Domain.Tags;
using Connect.Presentation.Profiles.DTO;
using Connect.Presentation.Profiles.Validators;
using NUnit.Framework;

namespace Connect.Presentation.Tests.Profiles.Validators;

public sealed class AddTagRequestValidatorTests
{
    private readonly AddTagRequestValidator validator = new();

    [TestCase("")]
    [TestCase(null)]
    public void EmptyName_ReturnsFillInATag(string? name)
    {
        var result = validator.Validate(new AddTagRequest(name));

        Assert.That(result.Errors.Single().ErrorMessage, Is.EqualTo("Fill in a tag"));
    }

    [Test]
    public void NameOf31Characters_ReturnsTheLengthMessage()
    {
        var result = validator.Validate(new AddTagRequest(new string('a', 31)));

        Assert.That(result.Errors.Single().ErrorMessage, Is.EqualTo("A tag has at most 30 characters"));
    }

    [Test]
    public void NameOf30Characters_IsAccepted()
    {
        Assert.That(validator.Validate(new AddTagRequest(new string('a', 30))).IsValid, Is.True);
    }

    [Test]
    public void NameWithSpaces_IsAcceptedOnceNormalised()
    {
        var result = validator.Validate(new AddTagRequest(TagRules.Normalize("  Drone   Video ")));

        Assert.That(result.IsValid, Is.True);
    }
}
