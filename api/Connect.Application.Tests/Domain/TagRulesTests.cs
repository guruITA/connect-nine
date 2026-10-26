using Connect.Domain.Tags;
using NUnit.Framework;

namespace Connect.Application.Tests.Domain;

public sealed class TagRulesTests
{
    [TestCase("football", "football")]
    [TestCase("Football", "football")]
    [TestCase("  Drone   Video ", "drone video")]
    [TestCase("\tVideo\nediting ", "video editing")]
    [TestCase("   ", "")]
    public void Normalize_TrimsJoinsSpacesAndLowercases(string name, string expected)
    {
        Assert.That(TagRules.Normalize(name), Is.EqualTo(expected));
    }

    [Test]
    public void Limits_AreThirtyCharactersAndTenTags()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(TagRules.MaxLength, Is.EqualTo(30));
            Assert.That(TagRules.MaxPerProfile, Is.EqualTo(10));
        }
    }
}
