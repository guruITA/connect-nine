using Connect.Presentation.Profiles.Validators;
using NUnit.Framework;

namespace Connect.Presentation.Tests.Profiles.Validators;

public sealed class SuggestedTagsQueryValidatorTests
{
    [Test]
    public void NoValues_AreThePageOneAndFifty()
    {
        string? problem = SuggestedTagsQueryValidator.Check(null, null, out int page, out int size);

        Assert.That((problem, page, size), Is.EqualTo(((string?)null, 1, 50)));
    }

    [TestCase("abc")]
    [TestCase("0")]
    [TestCase("-1")]
    public void WrongPage_ReturnsThePageMessage(string page)
    {
        Assert.That(
            SuggestedTagsQueryValidator.Check(page, null, out _, out _),
            Is.EqualTo("Use a page number of 1 or more"));
    }

    [TestCase("0")]
    [TestCase("51")]
    [TestCase("ten")]
    public void WrongPageSize_ReturnsThePageSizeMessage(string pageSize)
    {
        Assert.That(
            SuggestedTagsQueryValidator.Check("1", pageSize, out _, out _),
            Is.EqualTo("Use a page size from 1 to 50"));
    }
}
