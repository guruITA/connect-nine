using Connect.Domain.ParentConsents;
using Connect.Domain.Users;
using NUnit.Framework;

namespace Connect.Application.Tests.Domain;

public sealed class ParentConsentTests
{
    private static ParentConsent Consent(DateTimeOffset expiresAt)
    {
        return new ParentConsent
        {
            ProfileId = Guid.NewGuid(),
            ParentEmail = "parent@example.com",
            TokenHash = new string('a', 64),
            ExpiresAt = expiresAt
        };
    }

    [Test]
    public void IsUsable_OneSecondBeforeExpiresAt_IsTrue()
    {
        DateTimeOffset expiresAt = DateTimeOffset.UtcNow.AddMonths(ParentConsent.KeepMonths);
        ParentConsent consent = Consent(expiresAt);

        Assert.That(consent.IsUsable(expiresAt.AddSeconds(-1)), Is.True);
    }

    [Test]
    public void IsUsable_AtExpiresAt_IsFalse()
    {
        DateTimeOffset expiresAt = DateTimeOffset.UtcNow.AddMonths(ParentConsent.KeepMonths);
        ParentConsent consent = Consent(expiresAt);

        Assert.That(consent.IsUsable(expiresAt), Is.False);
    }

    [Test]
    public void IsUsable_AfterGive_IsFalse()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        ParentConsent consent = Consent(now.AddMonths(ParentConsent.KeepMonths));
        consent.Give(now);

        Assert.That(consent.IsUsable(now), Is.False);
    }

    [Test]
    public void IsUsable_EmptyAddress_IsFalse()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        ParentConsent consent = Consent(now.AddMonths(ParentConsent.KeepMonths));
        consent.ParentEmail = "";

        Assert.That(consent.IsUsable(now), Is.False);
    }

    [Test]
    public void Give_EmptiesTheParentAddress()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        ParentConsent consent = Consent(now.AddMonths(ParentConsent.KeepMonths));

        consent.Give(now);

        Assert.That(consent.ParentEmail, Is.Empty);
    }

    [Test]
    public void Give_Twice_KeepsTheFirstGivenAt()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        ParentConsent consent = Consent(now.AddMonths(ParentConsent.KeepMonths));

        consent.Give(now);
        consent.Give(now.AddDays(1));

        Assert.That(consent.GivenAt, Is.EqualTo(now));
    }

    [Test]
    public void AskParentConsent_KeepsTheRequestOneMonth()
    {
        var now = new DateTimeOffset(2026, 10, 23, 12, 0, 0, TimeSpan.Zero);
        var profile = new Profile
        {
            Account = new Account(),
            FirstName = "Noor",
            AgeBand = AgeBand.Adolescent,
            Region = "Uithoorn"
        };

        ParentConsent consent = profile.AskParentConsent("parent@example.com", new string('a', 64), now);

        Assert.That(consent.ExpiresAt, Is.EqualTo(new DateTimeOffset(2026, 11, 23, 12, 0, 0, TimeSpan.Zero)));
    }
}
