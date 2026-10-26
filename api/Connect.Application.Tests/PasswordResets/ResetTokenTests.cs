using System.Text.RegularExpressions;
using Connect.Application.PasswordResets;
using NUnit.Framework;

namespace Connect.Application.Tests.PasswordResets;

public sealed class ResetTokenTests
{
    [Test]
    public void Make_CalledTwice_ReturnsDifferentTokens()
    {
        string first = ResetToken.Make();
        string second = ResetToken.Make();

        Assert.That(first, Is.Not.EqualTo(second));
    }

    [Test]
    public void Make_ReturnsAUrlSafeBase64TokenOf32Bytes()
    {
        string token = ResetToken.Make();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(token, Does.Match("^[A-Za-z0-9_-]+$"));
            Assert.That(token.Length, Is.EqualTo(43));
        }
    }

    [Test]
    public void Hash_SameToken_ReturnsTheSameHash()
    {
        string token = ResetToken.Make();
        string first = ResetToken.Hash(token);
        string second = ResetToken.Hash(token);

        Assert.That(first, Is.EqualTo(second));
    }

    [Test]
    public void Hash_DifferentTokens_ReturnDifferentHashes()
    {
        string first = ResetToken.Hash(ResetToken.Make());
        string second = ResetToken.Hash(ResetToken.Make());

        Assert.That(first, Is.Not.EqualTo(second));
    }

    [Test]
    public void Hash_ReturnsSixtyFourLowerCaseHexCharacters()
    {
        string hash = ResetToken.Hash(ResetToken.Make());

        using (Assert.EnterMultipleScope())
        {
            Assert.That(hash.Length, Is.EqualTo(64));
            Assert.That(Regex.IsMatch(hash, "^[0-9a-f]{64}$"), Is.True);
        }
    }

    [Test]
    public void Hash_NeverEqualsTheTokenItself()
    {
        string token = ResetToken.Make();

        Assert.That(ResetToken.Hash(token), Is.Not.EqualTo(token));
    }
}
