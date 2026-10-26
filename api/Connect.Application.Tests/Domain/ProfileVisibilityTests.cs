using Connect.Domain.Users;
using NUnit.Framework;

namespace Connect.Application.Tests.Domain;

public sealed class ProfileVisibilityTests
{
    [Test]
    public void ShowsRegion_YoungPersonOf15To17_IsFalse()
    {
        Assert.That(ProfileVisibility.ShowsRegion(AgeBand.Adolescent), Is.False);
    }

    [TestCase(AgeBand.YoungAdult)]
    [TestCase(AgeBand.Adult)]
    public void ShowsRegion_Adult_IsTrue(AgeBand ageBand)
    {
        Assert.That(ProfileVisibility.ShowsRegion(ageBand), Is.True);
    }

    [TestCase(AgeBand.Adolescent)]
    [TestCase(AgeBand.YoungAdult)]
    [TestCase(AgeBand.Adult)]
    public void ShowsBio_EveryAgeBand_IsTrue(AgeBand ageBand)
    {
        Assert.That(ProfileVisibility.ShowsBio(ageBand), Is.True);
    }

    [TestCase(AgeBand.Adolescent)]
    [TestCase(AgeBand.YoungAdult)]
    [TestCase(AgeBand.Adult)]
    public void ShowsTags_EveryAgeBand_EqualsTheRuleForTheShortText(AgeBand ageBand)
    {
        Assert.That(ProfileVisibility.ShowsTags(ageBand), Is.EqualTo(ProfileVisibility.ShowsBio(ageBand)));
    }
}
