using Connect.Presentation.Accounts.DTO;
using Connect.Presentation.Accounts.Validators;
using NUnit.Framework;

namespace Connect.Presentation.Tests.Accounts.Validators;

public sealed class CreateAccountRequestValidatorTests
{
    private readonly CreateAccountRequestValidator validator = new();

    [Test]
    public void EmptyEmail_ReturnsTheEmailMessage()
    {
        var result = validator.Validate(new CreateAccountRequest("", "Valid-password-123!"));

        Assert.That(
            result.Errors.Single(error => error.PropertyName == nameof(CreateAccountRequest.Email))
                .ErrorMessage,
            Is.EqualTo("Fill in a valid email address"));
    }

    [Test]
    public void InvalidEmail_ReturnsTheEmailMessage()
    {
        var result = validator.Validate(new CreateAccountRequest("not-an-email", "Valid-password-123!"));

        Assert.That(
            result.Errors.Single(error => error.PropertyName == nameof(CreateAccountRequest.Email))
                .ErrorMessage,
            Is.EqualTo("Fill in a valid email address"));
    }

    [Test]
    public void EmptyPassword_ReturnsThePasswordMessage()
    {
        var result = validator.Validate(new CreateAccountRequest("member@example.com", ""));

        Assert.That(
            result.Errors.Single(error => error.PropertyName == nameof(CreateAccountRequest.Password))
                .ErrorMessage,
            Is.EqualTo("Fill in a password"));
    }

    [Test]
    public void PasswordMadeOfSpaces_IsRecognizedAsNonAlphanumeric()
    {
        var result = validator.Validate(new CreateAccountRequest(
            "member@example.com",
            "                "));

        Assert.That(
            result.Errors,
            Has.None.Matches<FluentValidation.Results.ValidationFailure>(
                error => error.ErrorMessage == "Password must contain at least one non-alphanumeric character"));
    }

    [Test]
    public void StrongPassword_IsValid()
    {
        var result = validator.Validate(new CreateAccountRequest(
            "member@example.com",
            "Valid-password-123!"));

        Assert.That(result.IsValid, Is.True);
    }
}
