using Connect.Presentation.Profiles.DTO;
using Connect.Presentation.Profiles.Validators;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace Connect.Presentation.Tests.Profiles.Validators;

public sealed class CreateMemberProfileRequestValidatorTests
{
    private readonly CreateMemberProfileRequestValidator validator = new();

    [Test]
    public void ValidRequest_HasNoErrors()
    {
        var result = validator.TestValidate(Valid());

        result.ShouldNotHaveAnyValidationErrors();
    }

    [TestCase("", "Fill in your first name")]
    [TestCase(null, "Fill in your first name")]
    public void MissingFirstName_ReturnsTheFirstNameMessage(string? firstName, string message)
    {
        var result = validator.TestValidate(Valid() with { FirstName = firstName });

        result.ShouldHaveValidationErrorFor(request => request.FirstName)
            .WithErrorMessage(message)
            .Only();
    }

    [Test]
    public void FirstNameOf60Characters_IsAccepted()
    {
        var result = validator.TestValidate(Valid() with { FirstName = new string('a', 60) });

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void FirstNameOf61Characters_ReturnsTheLengthMessage()
    {
        var result = validator.TestValidate(Valid() with { FirstName = new string('a', 61) });

        result.ShouldHaveValidationErrorFor(request => request.FirstName)
            .WithErrorMessage("Use at most 60 characters for your first name")
            .Only();
    }

    [TestCase("Adolescent")]
    [TestCase("YoungAdult")]
    [TestCase("Adult")]
    public void KnownAgeBand_IsAccepted(string ageBand)
    {
        var result = validator.TestValidate(Valid() with { AgeBand = ageBand });

        result.ShouldNotHaveAnyValidationErrors();
    }

    [TestCase("1")]
    [TestCase("youngadult")]
    [TestCase("16")]
    [TestCase("")]
    [TestCase(null)]
    public void UnknownAgeBand_ReturnsTheAgeBandMessage(string? ageBand)
    {
        var result = validator.TestValidate(Valid() with { AgeBand = ageBand });

        result.ShouldHaveValidationErrorFor(request => request.AgeBand)
            .WithErrorMessage("Choose your age band")
            .Only();
    }

    [TestCase("")]
    [TestCase(null)]
    public void MissingRegion_ReturnsTheRegionMessage(string? region)
    {
        var result = validator.TestValidate(Valid() with { Region = region });

        result.ShouldHaveValidationErrorFor(request => request.Region)
            .WithErrorMessage("Fill in your town or area")
            .Only();
    }

    [Test]
    public void RegionOf61Characters_ReturnsTheLengthMessage()
    {
        var result = validator.TestValidate(Valid() with { Region = new string('a', 61) });

        result.ShouldHaveValidationErrorFor(request => request.Region)
            .WithErrorMessage("Use at most 60 characters for your town or area")
            .Only();
    }

    [TestCase(null)]
    [TestCase("")]
    public void EmptyBio_IsAccepted(string? bio)
    {
        var result = validator.TestValidate(Valid() with { Bio = bio });

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void BioOf280Characters_IsAccepted()
    {
        var result = validator.TestValidate(Valid() with { Bio = new string('a', 280) });

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void BioOf281Characters_ReturnsTheLengthMessage()
    {
        var result = validator.TestValidate(Valid() with { Bio = new string('a', 281) });

        result.ShouldHaveValidationErrorFor(request => request.Bio)
            .WithErrorMessage("Your short line can be at most 280 characters")
            .Only();
    }

    [Test]
    public void EveryWrongField_ReturnsOneMessageEach()
    {
        var result = validator.TestValidate(new CreateMemberProfileRequest("", "1", "", new string('a', 281)));

        result.ShouldHaveValidationErrorFor(request => request.FirstName);
        result.ShouldHaveValidationErrorFor(request => request.AgeBand);
        result.ShouldHaveValidationErrorFor(request => request.Region);
        result.ShouldHaveValidationErrorFor(request => request.Bio);
        Assert.That(result.Errors, Has.Count.EqualTo(4));
    }

    private static CreateMemberProfileRequest Valid()
    {
        return new CreateMemberProfileRequest("Sam", "YoungAdult", "Amstelveen", "I like football.");
    }
}
