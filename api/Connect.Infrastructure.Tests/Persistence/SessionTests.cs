using Connect.Domain.Sessions;
using NUnit.Framework;

namespace Connect.Infrastructure.Tests.Persistence;

[TestFixture]
public sealed class SessionTests
{
    [TestCase(-1, ExpectedResult = true)]
    [TestCase(0, ExpectedResult = false)]
    [TestCase(1, ExpectedResult = false)]
    public bool IsActive_ReturnsExpectedResultAtExpiry(int secondsAfterExpiry)
    {
        DateTimeOffset expiresAt = DateTimeOffset.UtcNow;
        var session = new Session
        {
            AccountId = Guid.NewGuid(),
            TokenHash = new string('a', 64),
            ExpiresAt = expiresAt
        };

        return session.IsActive(expiresAt.AddSeconds(secondsAfterExpiry));
    }

    [Test]
    public void IsActive_ReturnsFalseAfterRevocation()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        var session = new Session
        {
            AccountId = Guid.NewGuid(),
            TokenHash = new string('a', 64),
            ExpiresAt = now.AddHours(1)
        };

        session.Revoke(now);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(session.RevokedAt, Is.EqualTo(now));
            Assert.That(session.IsActive(now), Is.False);
        }
    }

    [Test]
    public void Revoke_DoesNotReplaceExistingRevocationTime()
    {
        DateTimeOffset firstRevokedAt = DateTimeOffset.UtcNow;
        DateTimeOffset secondRevokedAt = firstRevokedAt.AddMinutes(1);
        var session = new Session
        {
            AccountId = Guid.NewGuid(),
            TokenHash = new string('a', 64),
            ExpiresAt = secondRevokedAt.AddHours(1)
        };

        session.Revoke(firstRevokedAt);
        session.Revoke(secondRevokedAt);

        Assert.That(session.RevokedAt, Is.EqualTo(firstRevokedAt));
    }
}
