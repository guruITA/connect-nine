using Connect.Application.Common.Results;
using NUnit.Framework;

namespace Connect.Application.Tests.Common.Results;

public sealed class ResultTests
{
    [Test]
    public void Success_ContainsValueAndNoError()
    {
        Result<string> result = Result<string>.Success("value");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo("value"));
            Assert.That(result.Error.Code, Is.EqualTo(ErrorCode.None));
        }
    }

    [Test]
    public void Failure_ContainsErrorAndNoValue()
    {
        Result<string> result = Result<string>.Failure(Error.Conflict("already exists"));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Value, Is.Null);
            Assert.That(result.Error.Code, Is.EqualTo(ErrorCode.Conflict));
        }
    }

    [Test]
    public void Failure_WithNoError_IsRejected()
    {
        Assert.That(
            () => Result<string>.Failure(Error.None),
            Throws.ArgumentException);
    }

    [Test]
    public void Success_WithNullValue_IsRejected()
    {
        Assert.That(
            () => Result<string>.Success(null!),
            Throws.ArgumentNullException);
    }
}
