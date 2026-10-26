using Connect.Domain.Users;
using NUnit.Framework;

namespace Connect.Application.Tests.Domain;

public sealed class ProfileVisibilityTests
{
    [Test]
    public void ShowsRegion_YoungPersonOf15To17_IsFalse()
    {
        Assert.That(ProfileVisibility.ShowsRegion(AgeBand.From15To17), Is.False);
    }

    [TestCase(AgeBand.From18To21)]
    [TestCase(AgeBand.From22To30)]
    public void ShowsRegion_Adult_IsTrue(AgeBand ageBand)
    {
        Assert.That(ProfileVisibility.ShowsRegion(ageBand), Is.True);
    }

    [TestCase(AgeBand.From15To17)]
    [TestCase(AgeBand.From18To21)]
    [TestCase(AgeBand.From22To30)]
    public void ShowsBio_EveryAgeBand_IsTrue(AgeBand ageBand)
    {
        Assert.That(ProfileVisibility.ShowsBio(ageBand), Is.True);
    }
}
