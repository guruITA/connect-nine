using Connect.Presentation.Accounts.DTO;
using Connect.Presentation.Accounts.Validators;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace Connect.Presentation.Tests.Accounts.Validators;

public sealed class CreateAccountRequestValidatorTests
{
    private readonly CreateAccountRequestValidator validator = new();

    [Test]
    public void EmptyEmail_ReturnsTheEmailMessage()
    {
        var result = validator.TestValidate(new CreateAccountRequest("", "Valid-password-123!"));

        result.ShouldHaveValidationErrorFor(request => request.Email)
            .WithErrorMessage("Fill in a valid email address")
            .Only();
    }

    [Test]
    public void InvalidEmail_ReturnsTheEmailMessage()
    {
        var result = validator.TestValidate(new CreateAccountRequest("not-an-email", "Valid-password-123!"));

        result.ShouldHaveValidationErrorFor(request => request.Email)
            .WithErrorMessage("Fill in a valid email address")
            .Only();
    }

    [Test]
    public void EmptyPassword_ReturnsThePasswordMessage()
    {
        var result = validator.TestValidate(new CreateAccountRequest("member@example.com", ""));

        result.ShouldHaveValidationErrorFor(request => request.Password)
            .WithErrorMessage("Fill in a password")
            .Only();
    }

    [Test]
    public void PasswordMadeOfSpaces_IsRecognizedAsNonAlphanumeric()
    {
        var result = validator.TestValidate(new CreateAccountRequest(
            "member@example.com",
            "                "));

        result.ShouldHaveValidationErrorFor(request => request.Password)
            .WithoutErrorMessage("Password must contain at least one non-alphanumeric character");
    }

    [Test]
    public void StrongPassword_IsValid()
    {
        var result = validator.TestValidate(new CreateAccountRequest(
            "member@example.com",
            "Valid-password-123!"));

        result.ShouldNotHaveAnyValidationErrors();
    }
}
