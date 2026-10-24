using Connect.Presentation.Profiles.DTO;
using Connect.Presentation.Profiles.Validators;
using NUnit.Framework;

namespace Connect.Presentation.Tests.Profiles.Validators;

public sealed class CreateMemberProfileRequestValidatorTests
{
    private readonly CreateMemberProfileRequestValidator validator = new();

    [Test]
    public void ValidRequest_HasNoErrors()
    {
        var result = validator.Validate(Valid());

        Assert.That(result.IsValid, Is.True);
    }

    [TestCase("", "Fill in your first name")]
    [TestCase(null, "Fill in your first name")]
    public void MissingFirstName_ReturnsTheFirstNameMessage(string? firstName, string message)
    {
        var result = validator.Validate(Valid() with { FirstName = firstName });

        Assert.That(MessageFor(result, nameof(CreateMemberProfileRequest.FirstName)), Is.EqualTo(message));
    }

    [Test]
    public void FirstNameOf60Characters_IsAccepted()
    {
        var result = validator.Validate(Valid() with { FirstName = new string('a', 60) });

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void FirstNameOf61Characters_ReturnsTheLengthMessage()
    {
        var result = validator.Validate(Valid() with { FirstName = new string('a', 61) });

        Assert.That(
            MessageFor(result, nameof(CreateMemberProfileRequest.FirstName)),
            Is.EqualTo("Use at most 60 characters for your first name"));
    }

    [TestCase("From15To17")]
    [TestCase("From18To21")]
    [TestCase("From22To30")]
    public void KnownAgeBand_IsAccepted(string ageBand)
    {
        var result = validator.Validate(Valid() with { AgeBand = ageBand });

        Assert.That(result.IsValid, Is.True);
    }

    [TestCase("1")]
    [TestCase("from18to21")]
    [TestCase("16")]
    [TestCase("")]
    [TestCase(null)]
    public void UnknownAgeBand_ReturnsTheAgeBandMessage(string? ageBand)
    {
        var result = validator.Validate(Valid() with { AgeBand = ageBand });

        Assert.That(
            MessageFor(result, nameof(CreateMemberProfileRequest.AgeBand)),
            Is.EqualTo("Choose your age band"));
    }

    [TestCase("")]
    [TestCase(null)]
    public void MissingRegion_ReturnsTheRegionMessage(string? region)
    {
        var result = validator.Validate(Valid() with { Region = region });

        Assert.That(
            MessageFor(result, nameof(CreateMemberProfileRequest.Region)),
            Is.EqualTo("Fill in your town or area"));
    }

    [Test]
    public void RegionOf61Characters_ReturnsTheLengthMessage()
    {
        var result = validator.Validate(Valid() with { Region = new string('a', 61) });

        Assert.That(
            MessageFor(result, nameof(CreateMemberProfileRequest.Region)),
            Is.EqualTo("Use at most 60 characters for your town or area"));
    }

    [TestCase(null)]
    [TestCase("")]
    public void EmptyBio_IsAccepted(string? bio)
    {
        var result = validator.Validate(Valid() with { Bio = bio });

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void BioOf280Characters_IsAccepted()
    {
        var result = validator.Validate(Valid() with { Bio = new string('a', 280) });

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void BioOf281Characters_ReturnsTheLengthMessage()
    {
        var result = validator.Validate(Valid() with { Bio = new string('a', 281) });

        Assert.That(
            MessageFor(result, nameof(CreateMemberProfileRequest.Bio)),
            Is.EqualTo("Use at most 280 characters about yourself"));
    }

    [Test]
    public void EveryWrongField_ReturnsOneMessageEach()
    {
        var result = validator.Validate(new CreateMemberProfileRequest("", "1", "", new string('a', 281)));

        Assert.That(
            result.Errors.Select(error => error.PropertyName),
            Is.EquivalentTo(new[] { "FirstName", "AgeBand", "Region", "Bio" }));
    }

    private static CreateMemberProfileRequest Valid()
    {
        return new CreateMemberProfileRequest("Sam", "From18To21", "Amstelveen", "I like football.");
    }

    private static string MessageFor(FluentValidation.Results.ValidationResult result, string property)
    {
        return result.Errors.Single(error => error.PropertyName == property).ErrorMessage;
    }
}
